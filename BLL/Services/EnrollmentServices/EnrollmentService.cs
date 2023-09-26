using BLL;
using BLL.Filter;
using BLL.LoggerService;
using BLL.StudentServices;
using BLL.Wrappers;
using DAL;
using Microsoft.AspNetCore.Http;
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
        private readonly ILoggerService logger;
        private readonly string smsClientId;
        public EnrollmentService(DataContext context, IStudentService studentService, IPaginationService paginationService, IHttpContextAccessor accessor, ILoggerService logger)
        {
            this.context = context;
            this.studentService = studentService;
            this.paginationService = paginationService;
            this.logger = logger;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> IEnrollmentService.GetEnrolledStudentsAsync(Guid sessionClassId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<EnrolledStudents>>>();
            try
            {
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
                var status = (int)EnrollmentStatus.Enrolled;

                var cls = context.SessionClass.Where(x => x.ClientId == smsClientId).Include(x => x.Session).FirstOrDefault(s => s.SessionClassId == sessionClassId);
                if (cls.Session.IsActive)
                    status = (int)EnrollmentStatus.Enrolled;
                else
                    status = (int)EnrollmentStatus.UnEnrolled;


                var query = (from a in context.StudentContact
                                    .Include(s => s.SessionClass)
                                    .Include(s => s.SessionClass).ThenInclude(s => s.Class)
                             where a.ClientId == smsClientId && a.Status == status && a.Deleted == false && a.SessionClassId == sessionClassId
                             select a);

                var totaltRecord = query.Count();
                var result = paginationService.GetPagedResult(query, filter).Select(a => new EnrolledStudents(a, regNoFormat)).ToList();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return await Task.Run(() => res);
            }
            catch(Exception ex)
            {
                logger.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        async Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> IEnrollmentService.GetUnenrrolledStudentsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<EnrolledStudents>>>();
            try
            {
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

                var query = (from a in context.StudentContact.Include(s => s.User)
                             where a.ClientId == smsClientId && a.EnrollmentStatus == (int)EnrollmentStatus.UnEnrolled && a.Deleted == false
                             select new EnrolledStudents
                             {
                                 Status = "unenrrolled",
                                 StudentContactId = a.StudentContactId.ToString(),
                                 StudentName = a.FirstName + " " + a.MiddleName + " " + a.LastName,
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
            catch (Exception ex) 
            {
                logger.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        async Task<APIResponse<Enroll>> IEnrollmentService.EnrollStudentsAsyncAsync(Enroll req)
        {
            var res = new APIResponse<Enroll>();

            foreach(var studentId in req.StudentContactIds)
            {
                try
                {
                    await studentService.ChangeClassAsync(Guid.Parse(studentId), Guid.Parse(req.SessionClassId));
                    var student = await context.StudentContact.FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                    if (student != null)
                    {
                        student.EnrollmentStatus = (int)EnrollmentStatus.Enrolled;
                        student.SessionClassId = Guid.Parse(req.SessionClassId);
                        student.Status = (int)StudentStatus.Active;
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                    return res;
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
                try
                {
                    var student = await context.StudentContact.FirstOrDefaultAsync(e => e.StudentContactId == Guid.Parse(studentId));
                    if (student != null)
                    {
                        student.EnrollmentStatus = (int)EnrollmentStatus.UnEnrolled;
                        student.Status = (int)EnrollmentStatus.UnEnrolled;
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                    return res;
                }
            }
            res.IsSuccessful = true;
            res.Result = req;
            res.Message.FriendlyMessage = req.StudentContactIds.Count() == 1 ? $"Successfuly unenrolled student" : $"Successfuly unenrolled students";
            return res;

        }

        void IEnrollmentService.UnenrollStudent(Guid studentId)
        {
            try
            {
                var enrollment = context.StudentContact.FirstOrDefault(e => e.StudentContactId == studentId);
                if (enrollment != null)
                {
                    enrollment.EnrollmentStatus = (int)EnrollmentStatus.UnEnrolled;
                    enrollment.Status = (int)StudentStatus.Inactive;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
            }
        }
    }
}
