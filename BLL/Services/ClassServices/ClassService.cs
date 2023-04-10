using BLL.Constants;
using BLL.LoggerService;
using Contracts.Class;
using Contracts.Options;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Services.SessionServices;
using SMP.BLL.Utilities;
using SMP.Contracts.ClassModels;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
namespace BLL.ClassServices
{
    public class ClassService : IClassService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IUtilitiesService utilitiesService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly ITermService termService;

        public ClassService(DataContext context,
            IHttpContextAccessor accessor, IUtilitiesService utilitiesService, 
            ILoggerService loggerService, ITermService termService)
        {
            this.context = context;
            this.accessor = accessor;
            this.utilitiesService = utilitiesService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
        }

        IQueryable<SessionClass> IClassService.GetSessionClass(Guid sessionClassId) => context.SessionClass
                .Include(x => x.Session)
                .Include(x => x.Class)
                .Where(x => x.SessionClassId == sessionClassId);

        ClassDto IClassService.GetSessionClassByLkp(Guid classLkp) => context.SessionClass
                .Where(x => x.ClientId == smsClientId && x.ClassId == classLkp && x.Deleted == false).Select(x => new ClassDto(x)).FirstOrDefault();

        async Task<APIResponse<SessionClassCommand2>> IClassService.CreateSessionClass2Async(SessionClassCommand2 sClass)
        {
            var res = new APIResponse<SessionClassCommand2>();
            if (context.SessionClass.Where(c => c.ClientId == smsClientId)
                .Include(x => x.Session)
                .Any(ss => ss.InSession == true && ss.ClassId == Guid.Parse(sClass.ClassId) && ss.Deleted == false &&
                        ss.SessionId == Guid.Parse(sClass.SessionId)))
            {
                res.Message.FriendlyMessage = "This class has already been added to this session";
                return res;
            }

            try
            {
                var sessionClass = new SessionClass
                {
                    ClassId = Guid.Parse(sClass.ClassId),
                    FormTeacherId = Guid.Parse(sClass.FormTeacherId),
                    SessionId = Guid.Parse(sClass.SessionId),
                    InSession = sClass.InSession,
                    ExamScore = sClass.ExamScore,
                    AssessmentScore = sClass.AssessmentScore,
                    PassMark = sClass.PassMark,
                    SessionTermId = termService.GetCurrentTerm().SessionTermId
                };
                context.SessionClass.Add(sessionClass);
                await context.SaveChangesAsync();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Session class created successfully";
                sClass.SessionClassId = sessionClass.SessionClassId.ToString();
                res.Result = sClass;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }


        async Task<APIResponse<SessionClassCommand>> IClassService.CreateSessionClassSubjectsAsync(ClassSubjectcommand request)
        {
            var res = new APIResponse<SessionClassCommand>();

            var sessionClass = context.SessionClass.Where(x => x.SessionClassId == request.SessionClassId && x.ClientId == smsClientId).Include(x => x.SessionClassSubjects).FirstOrDefault();
            if(sessionClass is null)
            {
                res.Message.FriendlyMessage = "Invalid Session class";
                return res;
            }

            if (!request.SubjectList.Any())
            {
                res.Message.FriendlyMessage = "No Subjects Selected";
                return res;
            }
            if (!request.SubjectList.All(e => !string.IsNullOrEmpty(e.SubjectId) && !string.IsNullOrEmpty(e.SubjectTeacherId)))
            {
                res.Message.FriendlyMessage = "Double check all selected subjects are mapped with subject teachers";
                return res;
            }

            await CreateUpdateClassSubjectsAsync(request.SubjectList, request.SessionClassId);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Updated successfully";
            return res;
        }

        async Task<APIResponse<SessionClassCommand>> IClassService.UpdateSessionClass2Async(SessionClassCommand2 request)
        {
            var res = new APIResponse<SessionClassCommand>();

            try
            {
                if (context.SessionClass.Where(c => c.ClientId == smsClientId)
                      .Include(x => x.Session)
                      .Any(ss => ss.Deleted == false && ss.ClassId == Guid.Parse(request.ClassId)
                      && ss.SessionClassId != Guid.Parse(request.SessionClassId)
                      && ss.SessionId == Guid.Parse(request.SessionId)))
                {
                    res.Message.FriendlyMessage = "This class has already been added to this session";
                    return res;
                }

                var sessionClass = context.SessionClass.FirstOrDefault(ss => ss.SessionClassId == Guid.Parse(request.SessionClassId) && ss.Deleted == false && ss.ClientId == smsClientId);

                if (sessionClass == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                sessionClass.FormTeacherId = Guid.Parse(request.FormTeacherId);
                sessionClass.InSession = request.InSession;
                sessionClass.ExamScore = request.ExamScore;
                sessionClass.AssessmentScore = request.AssessmentScore;
                sessionClass.PassMark = request.PassMark;
                await context.SaveChangesAsync();


                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Session class updated successfully";
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }


        private async Task CreateUpdateClassSubjectsAsync(ClassSubjects2[] ClassSubjects, Guid SessionClassId)
        {
            try
            {
                Guid[] selectedClassSubjectIds = ClassSubjects.Select(x => x.SubjectId).Select(Guid.Parse).ToArray();
                var deselectedSubjects = context.SessionClassSubject.Where(x => !selectedClassSubjectIds.Contains(x.SubjectId) && x.SessionClassId == SessionClassId).ToList();
                if (deselectedSubjects.Any()) context.RemoveRange(deselectedSubjects);
                context.SaveChanges();

                foreach (var subject in ClassSubjects)
                {
                    var sub = context.SessionClassSubject.FirstOrDefault(x => x.SubjectId == Guid.Parse(subject.SubjectId) && x.SessionClassId == SessionClassId);
                    if(sub is null)
                    {
                        sub = new SessionClassSubject();
                        sub.SessionClassId = SessionClassId;
                        sub.SubjectId = Guid.Parse(subject.SubjectId);
                        sub.SubjectTeacherId = Guid.Parse(subject.SubjectTeacherId);
                        sub.AssessmentScore = subject.Assessment;
                        sub.ExamScore = subject.ExamSCore; 
                        await context.SessionClassSubject.AddRangeAsync(sub);
                    }
                    else
                    {
                        sub.SessionClassId = SessionClassId;
                        sub.SubjectId = Guid.Parse(subject.SubjectId);
                        sub.SubjectTeacherId = Guid.Parse(subject.SubjectTeacherId);
                        sub.AssessmentScore = subject.Assessment;
                        sub.ExamScore = subject.ExamSCore;
                    }
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ;
            }
        }

        private async Task CreateClassSubjectsAsync(ClassSubjects2[] ClassSubjects, Guid SessionClassId)
        {
            try
            {
                var subs = new List<SessionClassSubject>();
                foreach (var subject in ClassSubjects)
                {
                    var sub = new SessionClassSubject();
                    sub.SessionClassId = SessionClassId;
                    sub.SubjectId = Guid.Parse(subject.SubjectId);
                    sub.SubjectTeacherId = Guid.Parse(subject.SubjectTeacherId);
                    sub.AssessmentScore = subject.Assessment;
                    sub.ExamScore = subject.ExamSCore;
                    subs.Add(sub);
                }
                await context.SessionClassSubject.AddRangeAsync(subs);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        private async Task DeleteExistingClassSubjectsAsync(Guid SessionClassId)
        {
            var subs = await context.SessionClassSubject.Where(sc => sc.SessionClassId == SessionClassId).ToListAsync();
            if (subs.Any())
                 context.SessionClassSubject.RemoveRange(subs);
            await context.SaveChangesAsync();
        }
        private async Task DeleteDeselectedClassSubjectsOnAsync(Guid SessionClassId, ClassSubjects[] subjects)
        {
            try
            {
                var subs = await context.SessionClassSubject.Include(x => x.SessionClassGroups)
               .Where(sc => sc.SessionClassId == SessionClassId && !subjects.Select(x => x.SubjectId)
               .AsEnumerable().Contains(sc.SubjectId.ToString())).ToListAsync();
               
                if (subs.Any())
                    context.SessionClassSubject.RemoveRange(subs);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw new ArgumentException("Please confirm home and class assessments are not in the deselected subjects");
            }
        }

        async Task<APIResponse<List<GetSessionClass>>> IClassService.GetSessionClassesAsync(string sessionId)
        {
            var res = new APIResponse<List<GetSessionClass>>();
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            //GET SUPER ADMIN CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClass.Where(c => c.ClientId == smsClientId)
                   .Include(rr => rr.Session)
                   .Include(rr => rr.Class)
                   .OrderBy(d => d.Class.Name)
                   .Where(r => r.Deleted == false && r.SessionId == Guid.Parse(sessionId))
                   .Include(rr => rr.Teacher).Select(g => new GetSessionClass(g)).ToListAsync();
                return res;
            }
            //GET TEACHER CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
            {
                var classesAsASujectTeacher = context.SessionClass.Where(c => c.ClientId == smsClientId)
                     .Include(s => s.Class)
                     .Include(s => s.Session)
                     .OrderBy(s => s.Class.Name)
                     .Where(e => e.Deleted == false && e.SessionId == Guid.Parse(sessionId) && e.SessionClassSubjects
                     .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                var classesAsAFormTeacher = context.SessionClass.Where(c => c.ClientId == smsClientId)
                    .Include(s => s.Class)
                    .Include(s => s.Session)
                    .OrderBy(s => s.Class.Name)
                    .Where(e => e.Deleted == false && e.SessionId == Guid.Parse(sessionId) && e.FormTeacherId == Guid.Parse(teacherId));
                res.Result = classesAsASujectTeacher.AsEnumerable().Concat(classesAsAFormTeacher.AsEnumerable()).Distinct().Select(s => new GetSessionClass(s)).ToList();
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetSessionClass>>> IClassService.GetSessionClasses1Async(string sessionId)
        {
            var res = new APIResponse<List<GetSessionClass>>();
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            //GET SUPER ADMIN CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClass.Where(c => c.ClientId == smsClientId)
                   .Include(rr => rr.Session)
                   .Include(rr => rr.Class)
                   .OrderBy(d => d.Class.Name)
                   .Where(r => r.Deleted == false && r.SessionId == Guid.Parse(sessionId))
                   .Include(rr => rr.Teacher).Select(g => new GetSessionClass(g)).ToListAsync();
                return res;
            }
            //GET TEACHER CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
            {
                var classesAsAFormTeacher = context.SessionClass.Where(c => c.ClientId == smsClientId)
                    .Include(s => s.Class)
                    .Include(s => s.Session)
                    .OrderBy(s => s.Class.Name)
                    .Where(e => e.Deleted == false && e.SessionId == Guid.Parse(sessionId) && e.FormTeacherId == Guid.Parse(teacherId));

                res.Result = classesAsAFormTeacher.AsEnumerable().Distinct().Select(s => new GetSessionClass(s)).ToList();
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<List<GetSessionClass>>> IClassService.GetSessionClasses2Async()
        {
            var res = new APIResponse<List<GetSessionClass>>();
            var sessionId = context.Session.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId).SessionId;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            //GET SUPER ADMIN CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClass.Where(c => c.ClientId == smsClientId)
                   .Include(rr => rr.Session)
                   .Include(rr => rr.Class)
                   .OrderBy(d => d.Class.Name)
                   .Where(r => r.Deleted == false && r.SessionId == sessionId)
                   .Include(r => r.ClassRegisters)
                   .Include(r => r.Students)
                   .Include(r => r.SessionClassSubjects).ThenInclude(x => x.ClassAssessments)
                    .Include(r => r.SessionClassSubjects).ThenInclude(x => x.HomeAssessments)
                   .Include(rr => rr.Teacher)
                   .Select(g => new GetSessionClass(g, true)).ToListAsync();
                return res;
            }
            //GET TEACHER CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
            {
                var classesAsASujectTeacher = context.SessionClass.Where(c => c.ClientId == smsClientId)
                     .Include(s => s.Class)
                     .Include(s => s.Session)
                     .Include(r => r.ClassRegisters)
                     .Include(r => r.Students)
                     .Include(r => r.SessionClassSubjects).ThenInclude(x => x.ClassAssessments)
                    .Include(r => r.SessionClassSubjects).ThenInclude(x => x.HomeAssessments)
                     .OrderBy(s => s.Class.Name)
                     .Where(e => e.Deleted == false && e.SessionId == sessionId && e.SessionClassSubjects 
                     .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                var classesAsAFormTeacher = context.SessionClass.Where(c => c.ClientId == smsClientId)
                    .Include(s => s.Class)
                    .Include(s => s.Session)
                    .Include(r => r.Students)
                    .OrderBy(s => s.Class.Name)
                    .Where(e => e.Deleted == false && e.SessionId == sessionId && e.FormTeacherId == Guid.Parse(teacherId));
                res.Result = classesAsASujectTeacher.AsEnumerable().Concat(classesAsAFormTeacher.AsEnumerable()).Distinct().Select(s => new GetSessionClass(s, true)).ToList();
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<GetSessionClass>> IClassService.GetSingleSessionClassesAsync(Guid sessionClassId)
        {
            var res = new APIResponse<GetSessionClass>();

            var result = await context.SessionClass.Where(r => r.InSession && sessionClassId == r.SessionClassId && r.Deleted == false && r.ClientId == smsClientId)
                .Include(rr => rr.Class)
                .Include(rr => rr.Session)
                .Include(rr => rr.Teacher).Select(g => new GetSessionClass(g)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<GetSessionClass>> IClassService.GetSingleSessionClassesWithoutSubjectsAndStudentsAsync(Guid sessionClassId)
        {
            var res = new APIResponse<GetSessionClass>();

            var result = await context.SessionClass.Where(r => r.InSession && sessionClassId == r.SessionClassId && r.Deleted == false && r.ClientId == smsClientId)
                .Include(rr => rr.Class)
                .Include(rr => rr.Session)
                .Include(rr => rr.Teacher).Select(g => new GetSessionClass(g)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<List<ClassSubjects>>> IClassService.GetSessionClassSubjects(Guid sessionClassId)
        {
            var res = new APIResponse<List<ClassSubjects>>();

            var result = await context.SessionClassSubject.Where(r =>  sessionClassId == r.SessionClassId && r.Deleted == false && r.ClientId == smsClientId)
                .Include(sub => sub.Subject)
                .Include(ses => ses.SubjectTeacher)
                .Select(g => new ClassSubjects(g)).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }


        async Task<APIResponse<List<GetStudentContacts>>> IClassService.GetClassStudentsClassesAsync(Guid sessionClassId)
        {
            var res = new APIResponse<List<GetStudentContacts>>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var result = await context.StudentContact.Where(x=>x.ClientId == smsClientId)
                .Include(x => x.User)
                .OrderByDescending(d => d.FirstName)
                .Where(d => d.Deleted == false && d.SessionClassId == sessionClassId && d.EnrollmentStatus == (int)EnrollmentStatus.Enrolled)
                .Select(f =>  new GetStudentContacts(f, regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetSessionClass>>> IClassService.GetSessionClassesBySessionAsync(string startDate, string endDate)
        {

            var res = new APIResponse<List<GetSessionClass>>();

            var query = context.SessionClass.Where(s=>s.ClientId == smsClientId).OrderByDescending(d => d.CreatedOn)
             .Include(rr => rr.Class)
             .Include(rr => rr.Session)
             .Include(rr => rr.Students)
             .Include(rr => rr.Teacher).Where(r => r.InSession);

            if (!string.IsNullOrEmpty(startDate))
                query = query.Where(v => v.Session.StartDate.Trim().ToLower() == startDate.Trim().ToLower());

            if (!string.IsNullOrEmpty(endDate))
                query = query.Where(v => v.Session.StartDate.Trim().ToLower() == endDate.Trim().ToLower());

            var result = await query.Select(g => new GetSessionClass(g)).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<bool>> IClassService.DeleteSessionClassesAsync(Guid sessionClassId)
        {
            var res = new APIResponse<bool>();

            var result = await context.SessionClass.Where(s=>s.ClientId == smsClientId).Include(x => x.Students).FirstOrDefaultAsync(r => sessionClassId == r.SessionClassId && r.Deleted == false);
            if(result == null)
            {
                res.Result = false;
                res.Message.FriendlyMessage = "Session class not found";
                return res;
            }

            if (result.Students.Any())
            {
                res.Result = false;
                res.Message.FriendlyMessage = "Class with students cannot be deleted";
                return res;
            }
            result.Deleted = true;
            await context.SaveChangesAsync();
            res.IsSuccessful = true;
            res.Result = true;
            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            return res;
        }

        public async Task<APIResponse<List<GetSessionClassCbt>>> GetSessionClassesCbtAsync(string clientId)
        {

            var res = new APIResponse<List<GetSessionClassCbt>>();
            try
            {
                var sessionId = context.Session.FirstOrDefault(x => x.IsActive && x.ClientId == clientId).SessionId;

                res.Result = await context.SessionClass.Where(c => c.ClientId == clientId)
                    .Include(rr => rr.Session)
                    .Include(rr => rr.Class)
                    .OrderBy(d => d.Class.Name)
                    .Where(r => r.Deleted == false && r.SessionId == sessionId)
                    .Select(g => new GetSessionClassCbt(g)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.Message;
                res.IsSuccessful = true;
                return res;
            }
        }

        public async Task<APIResponse<GetSessionClassCbt>> GetSessionClassesCbtByRegNoAsync(string registrationNo, string clientId)
        {
            var res = new APIResponse<GetSessionClassCbt>();
            try
            {
                string regNo = utilitiesService.GetStudentRegNumberValue(registrationNo, clientId);
                var student = await utilitiesService.GetStudentContactByRegNo(regNo, clientId);
                if (student == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Registration Number doesn't exists!";
                    return res;
                }

                var studentClass = await context.SessionClass.Where(x => x.Deleted == false && x.SessionClassId == student.SessionClassId && x.ClientId == clientId)
                    .Include(c => c.Session)
                    .Include(c => c.Class).Where(x=>x.Session.IsActive)
                    .Select(g => new GetSessionClassCbt(g)).FirstOrDefaultAsync();
                
                if (studentClass == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Student class doesn't exists!";
                    return res;
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = studentClass;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                return res;
            }
        }
    }
}
