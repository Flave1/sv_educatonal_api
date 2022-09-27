using BLL.Constants;
using BLL.Utilities;
using Contracts.Class;
using Contracts.Options;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.ResultServices;
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
        private readonly IResultsService resultsService;
        private readonly IHttpContextAccessor accessor;

        public ClassService(DataContext context, IResultsService resultsService, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.resultsService = resultsService;
            this.accessor = accessor;
        }

        async Task<APIResponse<SessionClassCommand>>  IClassService.CreateSessionClassAsync(SessionClassCommand sClass)
        {
            var res = new APIResponse<SessionClassCommand>();
            if (context.SessionClass
                .Include(x => x.Session)
                .Any(ss => ss.InSession == true && ss.ClassId == Guid.Parse(sClass.ClassId) && ss.Deleted == false && 
                        ss.SessionId == Guid.Parse(sClass.SessionId)))
            {
                res.Message.FriendlyMessage = "This class has already been added to this session";
                return res;
            }

            if (!sClass.ClassSubjects.Any())
            {
                res.Message.FriendlyMessage = "No Subjects found";
                return res;
            }
            if (!sClass.ClassSubjects.All(e => !string.IsNullOrEmpty(e.SubjectId) && !string.IsNullOrEmpty(e.SubjectTeacherId)))
            {
                res.Message.FriendlyMessage = "Double check all selected subjects are mapped with subject teachers";
                return res;
            }
          
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
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
                };
                    context.SessionClass.Add(sessionClass);
                    await context.SaveChangesAsync();

                    await CreateClassSubjectsAsync(sClass.ClassSubjects, sessionClass.SessionClassId);

                    await resultsService.CreateClassScoreEntryAsync(sessionClass);

                    await transaction.CommitAsync();
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Session class created successfully";
                    return res;

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = Messages.FriendlyException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }
            }
        }

        async Task<APIResponse<SessionClassCommand>> IClassService.UpdateSessionClassAsync(SessionClassCommand sClass)
        {
            var res = new APIResponse<SessionClassCommand>();

            try
            {

                if (context.SessionClass
                  .Include(x => x.Session)
                  .Any(ss => ss.Deleted == false && ss.ClassId == Guid.Parse(sClass.ClassId)
                  && ss.SessionClassId != Guid.Parse(sClass.SessionClassId)
                  && ss.SessionId == Guid.Parse(sClass.SessionId)))
                {
                    res.Message.FriendlyMessage = "This class has already been added to this session";
                    return res;
                }

                var sessionClass = context.SessionClass.FirstOrDefault(ss => ss.SessionClassId == Guid.Parse(sClass.SessionClassId) && ss.Deleted == false);

                if (sessionClass == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                if (!sClass.ClassSubjects.Any())
                {
                    res.Message.FriendlyMessage = "No Subjects found";
                    return res;
                }
                if (!sClass.ClassSubjects.All(e => !string.IsNullOrEmpty(e.SubjectId) && !string.IsNullOrEmpty(e.SubjectTeacherId)))
                {
                    res.Message.FriendlyMessage = "Double check all selected subjects are mapped with subject teachers";
                    return res;
                }

                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        sessionClass.FormTeacherId = Guid.Parse(sClass.FormTeacherId);
                        sessionClass.InSession = sClass.InSession;
                        sessionClass.ExamScore = sClass.ExamScore;
                        sessionClass.AssessmentScore = sClass.AssessmentScore;
                        sessionClass.PassMark = sClass.PassMark;
                        await context.SaveChangesAsync();

                        await DeleteDeselectedClassSubjectsOnAsync(sessionClass.SessionClassId, sClass.ClassSubjects);

                        await CreateUpdateClassSubjectsAsync(sClass.ClassSubjects, sessionClass.SessionClassId);

                        await resultsService.CreateClassScoreEntryAsync(sessionClass);

                        await transaction.CommitAsync();
                        res.IsSuccessful = true;
                        res.Message.FriendlyMessage = "Session class updated successfully";
                        return res;
                    }
                    //DbUpdateException
                    catch (ArgumentException ex)
                    {
                        await transaction.RollbackAsync();
                        res.Message.FriendlyMessage = ex.Message;
                        return res;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        res.Message.FriendlyMessage = Messages.FriendlyException;
                        res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }

           
        }

        private async Task CreateUpdateClassSubjectsAsync(ClassSubjects[] ClassSubjects, Guid SessionClassId)
        {
            try
            {
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

                throw ex;
            }
        }

        private async Task CreateClassSubjectsAsync(ClassSubjects[] ClassSubjects, Guid SessionClassId)
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
            catch (Exception)
            {
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
                throw new ArgumentException("Please confirm home and class assessments are not in the deselected subjects");
            }
        }

        async Task<APIResponse<List<GetSessionClass>>> IClassService.GetSessionClassesAsync(Guid sessionId)
        {
            var res = new APIResponse<List<GetSessionClass>>();
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            //GET SUPER ADMIN CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClass
                   .Include(rr => rr.Session)
                   .Include(rr => rr.Class)
                   .OrderBy(d => d.Class.Name)
                   .Where(r => r.Deleted == false && r.SessionId == sessionId)
                   .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Select(g => new GetSessionClass(g)).ToListAsync();
                return res;
            }
            //GET TEACHER CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
            {
                var classesAsASujectTeacher = context.SessionClass
                     .Include(s => s.Class)
                     .Include(s => s.Session)
                     .OrderBy(s => s.Class.Name)
                     .Where(e => e.Deleted == false && e.SessionId == sessionId && e.SessionClassSubjects 
                     .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                var classesAsAFormTeacher = context.SessionClass
                    .Include(s => s.Class)
                    .Include(s => s.Session)
                    .OrderBy(s => s.Class.Name)
                    .Where(e => e.Deleted == false && e.SessionId == sessionId && e.FormTeacherId == Guid.Parse(teacherId));
                res.Result = classesAsASujectTeacher.AsEnumerable().Concat(classesAsAFormTeacher.AsEnumerable()).Distinct().Select(s => new GetSessionClass(s)).ToList();
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetSessionClass>> IClassService.GetSingleSessionClassesAsync(Guid sessionClassId)
        {
            var res = new APIResponse<GetSessionClass>();

            var result = await context.SessionClass.Where(r => r.InSession && sessionClassId == r.SessionClassId && r.Deleted == false)
                .Include(rr => rr.Class)
                .Include(rr => rr.Session)
                .Include(rr => rr.Students)
                .Include(rr => rr.SessionClassSubjects).ThenInclude(sub => sub.Subject)
                .Include(rr => rr.SessionClassSubjects).ThenInclude(ses => ses.SubjectTeacher).ThenInclude(d => d.User)
                .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Select(g => new GetSessionClass(g)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<List<GetStudentContacts>>> IClassService.GetClassStudentsClassesAsync(Guid sessionClassId)
        {
            var res = new APIResponse<List<GetStudentContacts>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await context.StudentContact
                .Include(q => q.User)
                .OrderByDescending(d => d.User.FirstName)
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

            var query = context.SessionClass.OrderByDescending(d => d.CreatedOn)
             .Include(rr => rr.Class)
             .Include(rr => rr.Session)
             .Include(rr => rr.Students).ThenInclude(uuu => uuu.User)
             .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Where(r => r.InSession);

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

            var result = await context.SessionClass.FirstOrDefaultAsync(r => sessionClassId == r.SessionClassId && r.Deleted == false);
            if(result == null)
            {
                res.Result = false;
                res.Message.FriendlyMessage = "Session class not found";
                return res;
            }
            result.Deleted = true;
            await context.SaveChangesAsync();
            res.IsSuccessful = true;
            res.Result = true;
            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            return res;
        }
    }
}
