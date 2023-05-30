using BLL;
using BLL.LoggerService;
using BLL.SessionServices;
using BLL.StudentServices;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.EnrollmentServices;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.PromotionModels;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PromorionServices
{
    public class PromotionService : IPromotionService
    {
        private readonly DataContext context;
        private readonly IStudentService studentService;
        private readonly IResultsService resultsService;
        private readonly ISessionService sessionService;
        private readonly IEnrollmentService enrollmentService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;

        public PromotionService(DataContext context, IStudentService studentService, IResultsService resultsService, ISessionService sessionService, 
            IEnrollmentService enrollmentService, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.studentService = studentService;
            this.resultsService = resultsService;
            this.sessionService = sessionService;
            this.enrollmentService = enrollmentService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<List<PreviousSessionClasses>>> IPromotionService.GetPreviousSessionClassesAsync()
        {
            var res = new APIResponse<List<PreviousSessionClasses>>();

            var lastTwoSessions = await context.Session.Where(d => d.Deleted == false && d.ClientId == smsClientId).OrderByDescending(a => a.CreatedOn).Take(2).ToListAsync();

            if (lastTwoSessions.Any())
            {
                Guid previousSessionId = lastTwoSessions.Last().SessionId;
                var lastTermOfPreviousSession = sessionService.GetPreviousSessionLastTermAsync(previousSessionId);
                var classes = await context.SessionClass.Where(x => x.ClientId == smsClientId && x.InSession && x.Deleted == false && x.SessionId == previousSessionId)
                    .Include(rr => rr.Class)
                    .OrderBy(d => d.Class.Name)
                    .Select(g => new PreviousSessionClasses(g)).ToListAsync();

                foreach(var cl in classes)
                {
                    var studentRecords = context.StudentContact.Where(x => x.ClientId == smsClientId).Include(x => x.ScoreEntries).Where(x => x.SessionClassId == cl.SessionClassId).Select(d => new StudentResultRecord(d, lastTermOfPreviousSession.SessionTermId)).ToList();
                    if (studentRecords != null)
                    {
                        cl.PassedStudentIds = string.Join(',', studentRecords.Where(d => d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                        cl.FailedStudentIds = string.Join(',', studentRecords.Where(d => !d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                        cl.TotalStudentsPassed = studentRecords.Count(d => d.ShouldPromoteStudent);
                        cl.TotalStudentsFailed = studentRecords.Count(d => !d.ShouldPromoteStudent);
                        cl.TotalStudentsInClass = cl.TotalStudentsPassed + cl.TotalStudentsFailed;
                        cl.StudentsToBePromoted = context.SchoolSettings.Where(x => x.ClientId == smsClientId).FirstOrDefault().RESULTSETTINGS_PromoteAll == false ? cl.TotalStudentsInClass - cl.TotalStudentsFailed : cl.TotalStudentsInClass;
                    }
                }
                res.Result = classes.ToList();
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<bool>> IPromotionService.PromoteClassAsync(Promote request)
        {
            var res = new APIResponse<bool>();
            var passedStudents = !string.IsNullOrEmpty(request.PassedStudents) ? request.PassedStudents.Split(',').Select(Guid.Parse).ToList() : Enumerable.Empty<Guid>();
            var failedStudents = !string.IsNullOrEmpty(request.FailedStudents) ? request.FailedStudents.Split(',').Select(Guid.Parse).ToList() : Enumerable.Empty<Guid>();

            try
            {
                var resultSettings = context.SchoolSettings.Where(x => x.ClientId == smsClientId).FirstOrDefault();
                if (resultSettings == null)
                {
                    res.Message.FriendlyMessage = "Result settings not found";
                    return res;
                }

                SessionClass sessClass = context.SessionClass.Where(x => x.ClientId == smsClientId).FirstOrDefault(x => x.SessionClassId == Guid.Parse(request.ClassToBePromoted));

                if(sessClass == null) 
                {
                    loggerService.Debug($"Class not found on promotion: {smsClientId}");
                    res.Message.FriendlyMessage = "Class not found";
                    return res;
                }
                if (resultSettings.RESULTSETTINGS_PromoteAll)
                {
                    var allStudents = passedStudents.Concat(failedStudents).ToList();
                    foreach (var studentId in allStudents)
                    {
                        await studentService.ChangeClassAsync(studentId, Guid.Parse(request.ClassToPromoteTo));
                    }
                }
                else
                {
                    var lastTermOfPreviousSessionTerm = sessionService.GetPreviousSessionLastTermAsync(sessClass.SessionId.Value);
                    foreach (var studentId in passedStudents)
                    {
                        await studentService.ChangeClassAsync(studentId, Guid.Parse(request.ClassToPromoteTo));
                    }
                    foreach (var studentId in failedStudents)
                    {
                        enrollmentService.UnenrollStudent(studentId);
                    }

                }
                await UpdatePromotedClassAsync(sessClass);

                res.Message.FriendlyMessage = "Promotion Successful";
                res.Result = true;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.ClassTransitionException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

        }

        
        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllPassedStudentsAsync(FetchPassedOrFailedStudents request)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
            var ids = request.StudentIds.Split(',').ToArray();
            var result = await context.StudentContact.Where(x => x.ClientId == smsClientId && x.Deleted == false && ids.Contains(x.StudentContactId.ToString()))
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Select(f => new GetStudents(f, "passed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllFailedStudentsAsync(FetchPassedOrFailedStudents request)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
            var ids = request.StudentIds.Split(',').ToArray();
            var result = await context.StudentContact.Where(x=>x.ClientId == smsClientId && x.Deleted == false && ids.Contains(x.StudentContactId.ToString()))
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Select(f => new GetStudents(f, "failed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task UpdatePromotedClassAsync(SessionClass sessClass)
        {
            sessClass.IsPromoted = true;
            await context.SaveChangesAsync();
        }
    }
}
