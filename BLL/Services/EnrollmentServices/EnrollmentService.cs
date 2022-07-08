using BLL;
using BLL.StudentServices;
using BLL.Utilities;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
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
        public EnrollmentService(DataContext context, IStudentService studentService)
        {
            this.context = context;
            this.studentService = studentService;
        }

        async Task<APIResponse<List<EnrolledStudents>>> IEnrollmentService.GetAllEnrrolledStudentsAsync()
        {
            var res = new APIResponse<List<EnrolledStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await (from a in context.StudentContact
                                .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                                .Include(s => s.SessionClass).ThenInclude(s => s.Class).Include(s => s.User)
                          join b in context.Enrollment on a.StudentContactId equals b.StudentContactId
                          where b.Status == (int)EnrollmentStatus.Enrolled && a.SessionClass.Session.IsActive == true && a.SessionClassId == b.SessionClassId
                          select new EnrolledStudents
                          {
                              Status = "enrrolled",
                              SessionClassId = a.SessionClassId.ToString(),
                              StudentContactId = a.StudentContactId.ToString(),
                              StudentName = a.User.FirstName + " " + a.User.LastName,
                              StudentRegNumber = regNoFormat.Replace("%VALUE%", a.RegistrationNumber),
                              Class = a.SessionClass.Class.Name
                          }).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<EnrolledStudents>>> IEnrollmentService.GetAllUnenrrolledStudentsAsync()
        {
            var res = new APIResponse<List<EnrolledStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await (from a in context.StudentContact.Include(s => s.User)
                                join b in context.Enrollment on a.StudentContactId equals b.StudentContactId
                                where b.Status == (int)EnrollmentStatus.UnEnrolled
                                select new EnrolledStudents
                                {
                                    Status = "unenrrolled",
                                    StudentContactId = a.StudentContactId.ToString(),
                                    StudentName = a.User.FirstName + " " + a.User.LastName,
                                    StudentRegNumber = regNoFormat.Replace("%VALUE%", a.RegistrationNumber),
                                    Class = a.SessionClass.Class.Name
                                }).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
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

                        var enrollment = await context.Enrollment.FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                        if (enrollment != null)
                        {
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

                        var enrollment = await context.Enrollment.FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                        if (enrollment != null)
                        {
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
    
        //async Task IEnrollmentService.ReEnrollOnUpdateStudentAsync(StudentContact std)
        //{
        //    var enrollment = await context.Enrollment.FirstOrDefaultAsync(e => e.StudentId == std.StudentContactId);
        //    if(enrollment != null)
        //    {
        //        enrollment.StudentId = std.SessionClassId;
        //        enrollment.ClassId = std.SessionClassId;
        //        await context.SaveChangesAsync();
        //    }
        //}
    }
}
