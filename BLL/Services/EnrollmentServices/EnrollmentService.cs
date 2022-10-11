using BLL;
using BLL.Filter;
using BLL.StudentServices;
using BLL.Utilities;
using BLL.Wrappers;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Enrollment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.EnrollmentServices
{
    public class EnrollmentService: IEnrollmentService
    {
        private readonly DataContext context;
        private readonly IStudentService studentService;
        private readonly IPaginationService paginationService;
        public EnrollmentService(DataContext context, IStudentService studentService, IPaginationService paginationService)
        {
            this.context = context;
            this.studentService = studentService;
            this.paginationService = paginationService;
        }

        async Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> IEnrollmentService.GetEnrolledStudentsAsync(Guid sessionClassId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<EnrolledStudents>>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var status = (int)EnrollmentStatus.Enrolled;

            var cls = context.SessionClass.Include(x => x.Session).FirstOrDefault(s => s.SessionClassId == sessionClassId);
            if (cls.Session.IsActive)
                status = (int)EnrollmentStatus.Enrolled;
            else
                status = (int)EnrollmentStatus.UnEnrolled;


            var query = (from a in context.StudentContact
                                .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                                .Include(s => s.SessionClass).ThenInclude(s => s.Class).Include(s => s.User)
                          join b in context.Enrollment on a.StudentContactId equals b.StudentContactId
                          where b.Status == status && a.SessionClassId == sessionClassId select a);

            var totaltRecord = query.Count();
            var result = paginationService.GetPagedResult(query, filter).Select(a =>  new EnrolledStudents(a, regNoFormat)).ToList();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return await Task.Run(() =>  res);
        }

        async Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> IEnrollmentService.GetUnenrrolledStudentsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<EnrolledStudents>>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var query =  (from a in context.StudentContact.Include(s => s.User)
                                join b in context.Enrollment on a.StudentContactId equals b.StudentContactId
                                where b.Status == (int)EnrollmentStatus.UnEnrolled
                                select new EnrolledStudents
                                {
                                    Status = "unenrrolled",
                                    StudentContactId = a.StudentContactId.ToString(),
                                    StudentName = a.User.FirstName + " " + a.User.LastName,
                                    StudentRegNumber = regNoFormat.Replace("%VALUE%", a.RegistrationNumber),
                                    Class = a.SessionClass.Class.Name
                                });

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<Enroll>> IEnrollmentService.EnrollStudentsAsyncAsync(Enroll req)
        {
            var res = new APIResponse<Enroll>();

            foreach(var studentId in req.StudentContactIds)
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await studentService.ChangeClassAsync(Guid.Parse(studentId), Guid.Parse(req.SessionClassId));

                        var enrollment = await context.Enrollment.Include(d => d.Student).FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                        if (enrollment != null)
                        {
                            enrollment.Student.EnrollmentStatus = (int)EnrollmentStatus.Enrolled;
                            enrollment.StudentContactId = Guid.Parse(studentId);
                            enrollment.SessionClassId = Guid.Parse(req.SessionClassId);
                            enrollment.Status = (int)EnrollmentStatus.Enrolled;
                            await context.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        res.Message.FriendlyMessage = Messages.FriendlyException;
                        res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                        return res;
                    }
                    finally { await transaction.DisposeAsync(); }
                }

            }

            res.IsSuccessful = true;
            res.Result = req;
            res.Message.FriendlyMessage = req.StudentContactIds.Count() == 1 ? $"Successfuly enrolled student" : $"Successfuly enrolled students";
            return res;

        }

        async Task<APIResponse<UnEnroll>> IEnrollmentService.UnenrollStudentsAsyncAsync(UnEnroll req)
        {
            var res = new APIResponse<UnEnroll>();
            foreach (var studentId in req.StudentContactIds)
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var enrollment = await context.Enrollment.Include(d => d.Student).FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                        if (enrollment != null)
                        {
                            enrollment.Student.EnrollmentStatus = (int)EnrollmentStatus.UnEnrolled;
                            enrollment.Status = (int)EnrollmentStatus.UnEnrolled;
                            await context.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        res.Message.FriendlyMessage = Messages.FriendlyException;
                        res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                        return res;
                    }
                    finally { await transaction.DisposeAsync(); }
                }
            }

            res.IsSuccessful = true;
            res.Result = req;
            res.Message.FriendlyMessage = req.StudentContactIds.Count() == 1 ? $"Successfuly unenrolled student" : $"Successfuly unenrolled students";
            return res;

        }

        void IEnrollmentService.UnenrollStudent(Guid studentId)
        {
            var res = new APIResponse<UnEnroll>();
            try
            {
                var enrollment =  context.Enrollment.Include(d => d.Student).FirstOrDefault(e => e.StudentContactId == studentId);
                if (enrollment != null)
                {
                    enrollment.Student.EnrollmentStatus = (int)EnrollmentStatus.UnEnrolled;
                    enrollment.Status = (int)EnrollmentStatus.UnEnrolled;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
