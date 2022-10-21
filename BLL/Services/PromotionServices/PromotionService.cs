using BLL;
using BLL.Constants;
using BLL.SessionServices;
using BLL.StudentServices;
using BLL.Utilities;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.EnrollmentServices;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.PromotionModels;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public PromotionService(DataContext context, IStudentService studentService, IResultsService resultsService, ISessionService sessionService, IEnrollmentService enrollmentService)
        {
            this.context = context;
            this.studentService = studentService;
            this.resultsService = resultsService;
            this.sessionService = sessionService;
            this.enrollmentService = enrollmentService;
        }

        async Task<APIResponse<List<PreviousSessionClasses>>> IPromotionService.GetPreviousSessionClassesAsync()
        {
            var res = new APIResponse<List<PreviousSessionClasses>>();

            var lastTwoSessions = await context.Session.OrderByDescending(a => a.CreatedOn).Where(d => d.Deleted  == false).Take(2).ToListAsync();

            if (lastTwoSessions.Any())
            {
                Guid previousSessionId = lastTwoSessions.Last().SessionId;
                var lastTermOfPreviousSession = sessionService.GetPreviousSessionLastTermAsync(previousSessionId);
                var classes = await context.SessionClass
                    .Include(rr => rr.Class)
                    .Include(d => d.SessionClassArchive)
                    .OrderBy(d => d.Class.Name)
                    .Where(r => r.InSession && r.Deleted == false && r.SessionId == previousSessionId)
                    .Select(g => new PreviousSessionClasses(g)).ToListAsync();

                foreach(var cl in classes)
                {
                    var studentRecords = context.StudentContact.Include(x => x.ScoreEntries).Where(x => x.SessionClassId == cl.SessionClassId).Select(d => new StudentResultRecord(d, lastTermOfPreviousSession.SessionTermId)).ToList();
                    if (studentRecords != null)
                    {
                        cl.PassedStudentIds = string.Join(',', studentRecords.Where(d => d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                        cl.FailedStudentIds = string.Join(',', studentRecords.Where(d => !d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                        cl.TotalStudentsPassed = studentRecords.Count(d => d.ShouldPromoteStudent);
                        cl.TotalStudentsFailed = studentRecords.Count(d => !d.ShouldPromoteStudent);
                        cl.TotalStudentsInClass = cl.TotalStudentsPassed + cl.TotalStudentsFailed;
                        cl.StudentsToBePromoted = context.ResultSetting.FirstOrDefault().PromoteAll == false ? cl.TotalStudentsInClass - cl.TotalStudentsFailed : cl.TotalStudentsInClass;
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
                var resultSettings = context.ResultSetting.FirstOrDefault();
                if (resultSettings == null)
                {
                    res.Message.FriendlyMessage = "Result settings not found";
                    return res;
                }

                Guid session = context.SessionClass.FirstOrDefault(x => x.SessionClassId == Guid.Parse(request.ClassToBePromoted)).SessionId;

                if (resultSettings.PromoteAll)
                {
                    var allStudents = passedStudents.Concat(failedStudents).ToList();
                    foreach (var studentId in allStudents)
                    {
                        await studentService.ChangeClassAsync(studentId, Guid.Parse(request.ClassToPromoteTo));
                    }
                }
                else
                {
                    var lastTermOfPreviousSessionTerm = sessionService.GetPreviousSessionLastTermAsync(session);
                    foreach (var studentId in passedStudents)
                    {
                        await studentService.ChangeClassAsync(studentId, Guid.Parse(request.ClassToPromoteTo));
                    }
                    foreach (var studentId in failedStudents)
                    {
                        enrollmentService.UnenrollStudent(studentId);
                    }

                }
                await UpdatePromotedClassAsync(Guid.Parse(request.ClassToBePromoted));

                res.Message.FriendlyMessage = "Promotion Successful";
                res.Result = true;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = Messages.ClassTransitionException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

        }

        
        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllPassedStudentsAsync(FetchPassedOrFailedStudents request)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var ids = request.StudentIds.Split(',').ToArray();
            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Where(d => d.Deleted == false && ids.Contains(d.StudentContactId.ToString()))
                .Select(f => new GetStudents(f, "passed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllFailedStudentsAsync(FetchPassedOrFailedStudents request)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var ids = request.StudentIds.Split(',').ToArray();
            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Where(d => d.Deleted == false && ids.Contains(d.StudentContactId.ToString()))
                .Select(f => new GetStudents(f, "failed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task UpdatePromotedClassAsync(Guid sessionClassId)
        {
            var classToPrommote = await context.SessionClassArchive.FirstOrDefaultAsync(d => d.SessionClassId == sessionClassId);
            if (classToPrommote != null)
            {
                classToPrommote.IsPromoted = true;
                await context.SaveChangesAsync();
            }
            if(classToPrommote is null)
            {
                throw new ArgumentException("Invalid request on promotion");
            }
        }
    }
}
