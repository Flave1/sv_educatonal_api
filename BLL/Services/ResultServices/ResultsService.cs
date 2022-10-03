using BLL;
using BLL.Constants;
using BLL.Utilities;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public class ResultsService: IResultsService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public ResultsService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<List<GetClasses>>> IResultsService.GetCurrentStaffClassesAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value; 
            var res = new APIResponse<List<GetClasses>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClass
                        .Include(s => s.Class) 
                        .Include(s => s.Session)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false).Select(s => new GetClasses(s)).ToListAsync();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
                //GET TEACHER CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                   var classesAsASujectTeacher = context.SessionClass
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false && e.SessionClassSubjects
                        .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                    var classesAsAFormTeacher = context.SessionClass
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false && e.FormTeacherId == Guid.Parse(teacherId));


                    res.Result = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct().Select(s => new GetClasses(s)).ToList();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetClassSubjects>>> IResultsService.GetCurrentStaffClassSubjectsAsync(Guid sessionClassId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId= accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassSubjects>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClassSubject
                        .Include(d => d.Subject)
                        .Where(e => e.SessionClassId == sessionClassId
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s)).ToListAsync();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var subjectTeacherSubjects = context.SessionClassSubject
                        .Include(d => d.Subject)
                        .Where(e => e.SubjectTeacherId == Guid.Parse(teacherId) 
                        && e.SessionClassId == sessionClassId
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    var formTeacherSubjects = context.SessionClassSubject
                        .Include(d => d.Subject)
                        .Include(d => d.SessionClass)
                        .Where(e => e.SessionClassId == sessionClassId 
                        && e.SessionClass.FormTeacherId == Guid.Parse(teacherId)
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    res.Result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable()).Distinct().ToList();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetClassSubjects>>> IResultsService.GetCurrentStaffClassSubjects2Async(Guid classId, Guid sessionClassId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassSubjects>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClassSubject
                        .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Include(d => d.Subject)
                        .Where(e => e.SessionClass.ClassId == classId 
                        && e.SessionClass.Session.IsActive == true 
                        && e.SessionClassId == sessionClassId
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s)).ToListAsync();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var subjectTeacherSubjects = context.SessionClassSubject
                         .Include(x => x.SessionClass).ThenInclude(x => x.Session)
                        .Include(d => d.Subject)
                        .Where(e => e.SubjectTeacherId == Guid.Parse(teacherId) 
                        && e.SessionClass.ClassId == classId 
                        && e.SessionClass.Session.IsActive == true 
                        && e.SessionClassId == sessionClassId
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    var formTeacherSubjects = context.SessionClassSubject
                        .Include(d => d.Subject)
                        .Include(d => d.SessionClass).ThenInclude(x => x.Session)
                        .Where(e => e.SessionClass.ClassId == classId 
                        && e.SessionClass.FormTeacherId == Guid.Parse(teacherId) 
                        && e.SessionClass.Session.IsActive == true 
                        && e.SessionClassId == sessionClassId
                        && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new GetClassSubjects(s));

                    res.Result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable()).Distinct().ToList();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassScoreEntry>> IResultsService.GetClassEntryAsync(Guid sessionClassId, Guid subjectId)
        {
            var res = new APIResponse<GetClassScoreEntry>();
            var clas = context.SessionClass.Include(x => x.Session).FirstOrDefault(x => x.SessionClassId == sessionClassId);
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            if (clas.Session.IsActive)
            {
                var currentTerm = await context.SessionTerm.FirstOrDefaultAsync(d => d.IsActive);
                res.Result = await context.ClassScoreEntry
                   .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                   .Include(d => d.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                   .Include(d => d.SessionClass).ThenInclude(d => d.SessionClassSubjects).ThenInclude(e => e.SubjectTeacher).ThenInclude(x => x.User)
                   .Include(d => d.Subject)
                   .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                   .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new GetClassScoreEntry(s, regNoFormat, currentTerm)).FirstOrDefaultAsync();
            }
            else
            {
                var clasArchive = context.SessionClassArchive.FirstOrDefault(x => x.SessionClassId == sessionClassId);
                res.Result = await context.ClassScoreEntry
                   .Include(d => d.SessionClass).ThenInclude(d => d.SessionClassSubjects).ThenInclude(e => e.SubjectTeacher).ThenInclude(x => x.User)
                   .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                   .Include(d => d.Subject)
                   .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                   .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new GetClassScoreEntry(s, regNoFormat, clasArchive.SessionTermId.Value)).FirstOrDefaultAsync();
           
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PreviewClassScoreEntry>> IResultsService.PreviewClassScoreEntry(Guid sessionClassId, Guid subjectId)
        {
            var res = new APIResponse<PreviewClassScoreEntry>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            res.Result = await context.ClassScoreEntry
                .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                .Include(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                .Include(d => d.Subject)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.SessionTerm)
                .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new PreviewClassScoreEntry(s, regNoFormat)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task IResultsService.CreateClassScoreEntryAsync(SessionClass sessionClass)
        {
            try
            {
                var selectedClassSubjectIds = sessionClass.SessionClassSubjects.Select(d => d.SubjectId);

                var deSelectedSubjects = context.ClassScoreEntry.Where(e => e.SessionClassId == sessionClass.SessionClassId && !selectedClassSubjectIds.Contains(e.SubjectId)).ToList();
                if (deSelectedSubjects.Any()) context.RemoveRange(deSelectedSubjects);

                foreach (var subject in sessionClass.SessionClassSubjects)
                {
                    var classEntry = context.ClassScoreEntry.FirstOrDefault(d => d.SessionClassId == sessionClass.SessionClassId && subject.SubjectId == d.SubjectId);
                    if (classEntry == null)
                    {
                        classEntry = new ClassScoreEntry();
                        classEntry.SessionClassId = sessionClass.SessionClassId;
                        classEntry.SubjectId = subject.SubjectId;
                        await context.ClassScoreEntry.AddAsync(classEntry);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdateExamScore(UpdateScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                var selectedTerm = context.SessionTerm.FirstOrDefault(d => Guid.Parse(request.TermId) == d.SessionTermId);
                if (selectedTerm != null)
                {
                    var classEntry = await context.ClassScoreEntry.Include(d => d.ScoreEntries)
                        .Where(a => a.ClassScoreEntryId == Guid.Parse(request.ClassScoreEntryId)).Select(s => s.ScoreEntries).FirstOrDefaultAsync();


                     var entry = classEntry.FirstOrDefault(a => a.SessionTermId == selectedTerm.SessionTermId && a.StudentContactId == Guid.Parse(request.StudentContactId));

                    if (entry != null)
                    {
                        entry.ExamScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        context.Entry(entry).CurrentValues.SetValues(entry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        entry = new ScoreEntry();
                        entry.ExamScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.SessionTermId = selectedTerm.SessionTermId;
                        entry.StudentContactId = Guid.Parse(request.StudentContactId);
                        entry.ClassScoreEntryId = Guid.Parse(request.ClassScoreEntryId);
                        await context.AddAsync(entry);
                        await context.SaveChangesAsync();
                    }
                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                    res.Message.FriendlyMessage = "System facing some technical issues with running term";
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdateAssessmentScore(UpdateScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                var selectedTerm = context.SessionTerm.FirstOrDefault(d => Guid.Parse(request.TermId) == d.SessionTermId);
                if (selectedTerm != null)
                {
                    var classEntry = await context.ClassScoreEntry.Include(d => d.ScoreEntries)
                        .Where(a => a.ClassScoreEntryId == Guid.Parse(request.ClassScoreEntryId)).Select(s => s.ScoreEntries).FirstOrDefaultAsync();


                    var entry = classEntry.FirstOrDefault(a => a.SessionTermId == selectedTerm.SessionTermId && a.StudentContactId == Guid.Parse(request.StudentContactId));

                    if (entry != null)
                    {
                        entry.AssessmentScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        context.Entry(entry).CurrentValues.SetValues(entry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        entry = new ScoreEntry();
                        entry.AssessmentScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.SessionTermId = selectedTerm.SessionTermId;
                        entry.StudentContactId = Guid.Parse(request.StudentContactId);
                        entry.ClassScoreEntryId = Guid.Parse(request.ClassScoreEntryId);
                        await context.AddAsync(entry);
                        await context.SaveChangesAsync();
                    }
                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                    res.Message.FriendlyMessage = "System facing some technical issues with running term";
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }
       
        async Task<APIResponse<MasterList>> IResultsService.GetMasterListAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();
            var res = new APIResponse<MasterList>();

            var clas = await context.SessionClass.Include(x => x.Session)
                .Include(x => x.Class)
                .Include(x => x.Teacher).ThenInclude(r => r.User)
                .Where(x => x.SessionClassId == sessionClassId).FirstOrDefaultAsync();

            var mlist = new MasterList(clas, term);

           var result = context.ScoreEntry
                    .Include(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                    .Include(r => r.StudentContact).ThenInclude(d => d.User)
                    .Include(r => r.ClassScoreEntry).ThenInclude(x => x.SessionClass).ThenInclude(x => x.Class)
                    .Where(r => r.ClassScoreEntry.SessionClassId == sessionClassId && r.SessionTermId == termId).AsEnumerable().GroupBy(x => x.StudentContactId)
                    .Select(g => new MasterListResult(g, regNoFormat)).ToList();

            if (result.Any())
            {
                var averages = result.Select(d => d.AverageScore);
                var studentPositions = UtilTools.GetStudentPositions(averages);
                foreach(var item in result)
                {
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position?? "";
                }

                result = result.OrderByDescending(d => d.AverageScore).ToList();
                mlist.ResultList = result;
            }
            res.IsSuccessful = true;
            res.Result = mlist;
            return res;
        }

        async Task<APIResponse<StudentResult>> IResultsService.GetClassResultListAsync(Guid sessionClassId, Guid termId)
        {
            var res = new APIResponse<StudentResult>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            try
            {

                var clas = context.SessionClass.Include(x => x.Session).FirstOrDefault(d => d.SessionClassId == sessionClassId);
                var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();
                res.Result = new StudentResult();
                if (clas.Session.IsActive)
                {
                    var result = await context.StudentContact
                       .Include(d => d.User)
                       .Include(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry)
                       .Where(rr => rr.SessionClassId == sessionClassId)
                       .Select(g => new StudentResultDetail(g, regNoFormat, sessionClassId, term.SessionTermId)).ToListAsync();

                    if (result != null)
                    {
                        var averages = result.Select(d => d.AverageScore);
                        var studentPositions = UtilTools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";
                        }
                        result = result.OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.IsPublished  = IsResultPublished(sessionClassId, termId);
                        res.Result.PublishResult = result;
                    }
                    res.IsSuccessful = true;
                    return res;
                }
                else
                {
                    var classArchive = context.SessionClassArchive.Where(d => d.SessionClassId == sessionClassId && d.SessionTermId == termId).ToList();
                    if (!classArchive.Any())
                    {
                        res.Message.FriendlyMessage = "Result for this session and term was not published";
                        return res;
                    }

                    var result = context.ScoreEntry
                    .Include(e => e.ClassScoreEntry).ThenInclude(x => x.SessionClass)
                    .Include(d => d.StudentContact).ThenInclude(d => d.User)
                    .Where(rr => rr.ClassScoreEntry.SessionClassId == sessionClassId && termId == rr.SessionTermId).AsEnumerable().GroupBy(s => s.StudentContactId)
                    .Select(entries => new StudentResultDetail(entries, regNoFormat)).ToList();

                    if (result != null)
                    {
                        var averages = result.Select(x => x.AverageScore);
                        var studentPositions = UtilTools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == (decimal)item.AverageScore)?.Position ?? "";
                        }
                        res.Result.PublishResult = result.OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.IsPublished = IsResultPublished(sessionClassId, termId);
                    }
                    res.IsSuccessful = true;
                    //res.Result = result;
                    return res;
                }


            }
            catch (Exception)
            {
                throw;
            }
           
            
        }

        async Task<APIResponse<PublishResultRequest>> IResultsService.PublishResultAsync(PublishResultRequest request)
        {
            var res = new APIResponse<PublishResultRequest>();
            try
            {
                var sessClass = await context.SessionClass.Include(x => x.Session).Include(x => x.Students)
                    .FirstOrDefaultAsync(d => d.SessionClassId == request.SessionClassId);

                if (sessClass.Session.IsActive)
                {
                    foreach (var student in sessClass.Students.Where(d => d.EnrollmentStatus == (int)Constants.EnrollmentStatus.Enrolled))
                    {
                        await SaveSessionClassArchiveAsync(sessClass.SessionClassId, request.SessionTermId, student.StudentContactId, request.Publish);
                    }
                   
                }
                else
                {
                    foreach (var student in context.SessionClassArchive.Where(d => d.SessionClassId == request.SessionClassId).ToList())
                    {
                        await (this as IResultsService).UpdateSessionClassArchiveAsync(student.StudentContactId.Value, student.SessionTermId.Value, request.Publish);
                    }
                }
                await context.SaveChangesAsync();

                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = request.Publish ? $"You have Successfully published student results" : $"You have Successfully Unpublished student results";
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                res.Message.TechnicalMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<GetClassScoreEntry>> IResultsService.GetPreviousTermsClassSubjectScoreEntriesAsync(Guid sessionClassId, Guid subjectId, Guid sessionTermId)
        {
            var res = new APIResponse<GetClassScoreEntry>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var clas = context.SessionClass.Include(s => s.Session).ThenInclude(x => x.Terms).FirstOrDefault(x => x.SessionClassId == sessionClassId);
            if (clas.Session.IsActive)
            {
                var term = clas.Session.Terms.FirstOrDefault(x => x.IsActive == true);
                res.Result = await context.ClassScoreEntry
                   .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                   .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                   .Include(d => d.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                   .Include(d => d.Subject)
                   .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                   .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId)
                   .Select(s => new GetClassScoreEntry(s, regNoFormat, term)).FirstOrDefaultAsync();
            }
            else
            {
                res.Result = await context.ClassScoreEntry
                  .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                  .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                  .Include(d => d.Subject)
                  .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                  .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId)
                  .Select(s => new GetClassScoreEntry(s, regNoFormat, sessionTermId)).FirstOrDefaultAsync();

            }
           



            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdatePreviousTermsExamScore(UpdateOtherSessionScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                var currentTerm = context.SessionTerm.FirstOrDefault(d => d.SessionTermId == Guid.Parse(request.SessionTermId));
                if (currentTerm != null)
                {
                    var classEntry = await context.ClassScoreEntry.Include(d => d.ScoreEntries)
                        .Where(a => a.ClassScoreEntryId == Guid.Parse(request.ClassScoreEntryId)).Select(s => s.ScoreEntries).FirstOrDefaultAsync();

                    var entry = classEntry.FirstOrDefault(a => a.SessionTermId == currentTerm.SessionTermId && a.StudentContactId == Guid.Parse(request.StudentContactId));

                    if (entry != null)
                    {
                        entry.ExamScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        context.Entry(entry).CurrentValues.SetValues(entry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        entry = new ScoreEntry();
                        entry.ExamScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.SessionTermId = currentTerm.SessionTermId;
                        entry.StudentContactId = Guid.Parse(request.StudentContactId);
                        entry.ClassScoreEntryId = Guid.Parse(request.ClassScoreEntryId);
                        await context.AddAsync(entry);
                        await context.SaveChangesAsync();
                    }
                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<ScoreEntry>> IResultsService.UpdatePreviousTermsAssessmentScore(UpdateOtherSessionScore request)
        {
            var res = new APIResponse<ScoreEntry>();
            try
            {
                var currentTerm = context.SessionTerm.FirstOrDefault(d => d.SessionTermId == Guid.Parse(request.SessionTermId));
                if (currentTerm != null)
                {
                    var classEntry = await context.ClassScoreEntry.Include(d => d.ScoreEntries)
                        .Where(a => a.ClassScoreEntryId == Guid.Parse(request.ClassScoreEntryId)).Select(s => s.ScoreEntries).FirstOrDefaultAsync();


                    var entry = classEntry.FirstOrDefault(a => a.SessionTermId == currentTerm.SessionTermId && a.StudentContactId == Guid.Parse(request.StudentContactId));

                    if (entry != null)
                    {
                        entry.AssessmentScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        context.Entry(entry).CurrentValues.SetValues(entry);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        entry = new ScoreEntry();
                        entry.AssessmentScore = request.Score;
                        entry.IsSaved = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.IsOffered = entry.ExamScore > 0 || entry.AssessmentScore > 0;
                        entry.SessionTermId = currentTerm.SessionTermId;
                        entry.StudentContactId = Guid.Parse(request.StudentContactId);
                        entry.ClassScoreEntryId = Guid.Parse(request.ClassScoreEntryId);
                        await context.AddAsync(entry);
                        await context.SaveChangesAsync();
                    }
                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }

        async Task<APIResponse<PreviewClassScoreEntry>> IResultsService.PreviewPreviousTermsClassScoreEntry(Guid sessionClassId, Guid subjectId, Guid sessionTermId)
        {
            var res = new APIResponse<PreviewClassScoreEntry>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            res.Result = await context.ClassScoreEntry
                .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                .Include(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                .Include(d => d.Subject)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.SessionTerm)
                .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new PreviewClassScoreEntry(s, regNoFormat, sessionTermId)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<CumulativeMasterList>> IResultsService.GetCumulativeMasterListAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            var res = new APIResponse<CumulativeMasterList>();
            var clas = await context.SessionClass.Include(r => r.Class)
                .Include(x => x.Session)
                .Include(r => r.Teacher).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(d => d.SessionClassId == sessionClassId);
            var cMList = new CumulativeMasterList(clas);

            var result =  context.ScoreEntry
                     .Include(s => s.SessionTerm)
                     .Include(r => r.StudentContact).ThenInclude(d => d.User)
                     .Include(d => d.ClassScoreEntry).ThenInclude(r => r.SessionClass).ThenInclude(r => r.ClassScoreEntries).ThenInclude(d => d.Subject)
                     .Where(r => r.ClassScoreEntry.SessionClassId == sessionClassId).AsEnumerable().GroupBy(s => s.StudentContactId)
                     .Select(g => new CumulativeMasterListResult(g, regNoFormat)).ToList();

            if (result.Any())
            {
                var averages = result.Select(d => d.AverageScore);
                var studentPositions = UtilTools.GetStudentPositions(averages);

                foreach (var item in result)
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";

                result = result.OrderByDescending(d => d.AverageScore).ToList();
                cMList.ResultList = result;
            }
            res.IsSuccessful = true;
            res.Result = cMList;
            return res;
        }

        async Task<APIResponse<StudentCoreEntry>> IResultsService.GetSingleStudentScoreEntryAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var res = new APIResponse<StudentCoreEntry>();

            var clas = context.SessionClass.Include(x => x.Class).Include(x => x.Session).FirstOrDefault(d => d.SessionClassId == sessionClassId);
            if (clas.Session.IsActive)
            {
                res.Result = await context.SessionClass
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(x => x.SessionTerm)
                 .Include(r => r.Students).ThenInclude(d => d.User)
                 .Include(r => r.Students).ThenInclude(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                 .Where(r => r.SessionClassId == sessionClassId)
                 .Select(g => new StudentCoreEntry(g.Students.FirstOrDefault(x => x.StudentContactId == studentContactId), regNoFormat, termId)).FirstOrDefaultAsync();

                if (res.Result != null)
                    res.Result.IsPublished = IsResultPublished(sessionClassId, termId, studentContactId);
            }
            else
            {
                var student = context.StudentContact.Include(x => x.User).FirstOrDefault(s => s.StudentContactId == studentContactId);
                var studentResult = new StudentCoreEntry(student, regNoFormat);
                studentResult.SessionClassName = clas.Class.Name;
                var result = await context.ScoreEntry
                    .Include(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                    .Include(x => x.SessionTerm)
                    .Include(x => x.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                    .Where(r => r.ClassScoreEntry.SessionClassId == sessionClassId && r.StudentContactId == studentContactId && r.SessionTermId == termId)
                    .Select(g => new StudentSubjectEntries(g, g.ClassScoreEntry.SessionClass.Class.GradeLevel)).ToListAsync();

                studentResult.StudentSubjectEntries = result;
                res.Result = studentResult;

                if (res.Result != null)
                    res.Result.IsPublished = IsResultPublished(sessionClassId, termId, studentContactId);
            }
                

          
            res.IsSuccessful = true;
            return res;
        }

       
        async Task<StudentResultRecord> IResultsService.GetStudentResultOnPromotionAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();

            var result = await context.SessionClass
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(x => x.SessionTerm)
                 .Where(r => r.SessionClassId == sessionClassId)
                 .Select(g => new StudentResultRecord(g.Students.FirstOrDefault(x => x.StudentContactId == studentContactId), termId)).FirstOrDefaultAsync();

            return result;
        }

        async Task<StudentResultRecord> IResultsService.GetStudentResultOnPromotionAsync(Guid sessionClassId, Guid termId)
        {
            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();

            var result = await context.SessionClass
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(x => x.SessionTerm)
                 .Where(r => r.SessionClassId == sessionClassId)
                 .Select(g => new StudentResultRecord(g.Students, termId)).FirstOrDefaultAsync();

            return result;
        }
    
        private async Task SaveSessionClassArchiveAsync(Guid classId, Guid termId, Guid studentId, bool publish)
        {
            var archive = await context.SessionClassArchive
                .FirstOrDefaultAsync(x => x.SessionClassId == classId && termId == x.SessionTermId && x.StudentContactId == studentId);

            if(archive is null)
            {
                archive = new SessionClassArchive
                {
                    HasPrintedResult = false,
                    SessionClassId = classId,
                    StudentContactId = studentId,
                    SessionTermId = termId,
                    IsPublished = publish
                };
                await context.SessionClassArchive.AddAsync(archive);
            }
            else
                archive.IsPublished = publish;
        }

        async Task IResultsService.UpdateSessionClassArchiveAsync(Guid studentId, Guid termId, bool isPublished)
        {
            var archive = await context.SessionClassArchive
                .FirstOrDefaultAsync(x => x.SessionTermId == termId && x.StudentContactId == studentId);
            if (archive is not null)
            {
                archive.IsPublished = isPublished;
            }
            else
            {
                throw new ArgumentException("This sudent result has not been updated!! Helpful tip to fix this, republish this class result to capture this student");
            }
        }
        async Task IResultsService.UpdateStudentPrintStatusAsync(Guid studentId, Guid termId, bool isResultPrinted)
        {
            var archive = await context.SessionClassArchive
                .FirstOrDefaultAsync(x => x.SessionTermId == termId && x.StudentContactId == studentId);
            if (archive is not null)
            {
                archive.HasPrintedResult = isResultPrinted;
            }
            else
            {
                throw new ArgumentException("This sudent result has not been updated!! Helpful tip to fix this, republish this class result to capture this student");
            }
        }

        async Task<APIResponse<List<PrintResult>>> IResultsService.GetStudentResultForBatchPrintingAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var res = new APIResponse<List<PrintResult>>();
            try
            {
                var term = await context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefaultAsync();
                if (term == null)
                {
                    res.Message.FriendlyMessage = "Term not found";
                    return res;
                }

                var results =  context.ScoreEntry
                   .Include(d => d.StudentContact).ThenInclude(d => d.User)
                   .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Session)
                    .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(s => s.GradeLevel).ThenInclude(x => x.Grades)
                   .Include(r => r.ClassScoreEntry).ThenInclude(s => s.Subject)
                   .Where(rr => rr.ClassScoreEntry.SessionClassId == sessionClassId && rr.SessionTermId == termId).AsEnumerable()
                   .GroupBy(d => d.StudentContactId, (Key, g) => new { studentId = Key, lst = g.ToList()})
                   .Select(s => new PrintResult(s.lst, regNoFormat, term))
                   .ToList() ?? new List<PrintResult>();


                if (results.Any())
                {
                    var averages = results.Select(d => d.average);
                    var studentPositions = UtilTools.GetStudentPositions(averages);
                    foreach(var std in results)
                    {
                        std.position = studentPositions.FirstOrDefault(d => d.Average == std.average)?.Position ?? "";
                        std.noOfStudents = results.Count();
                    } 
                    res.Result = results;
                }
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<PrintResult>> IResultsService.GetStudentResultForPrintingAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var res = new APIResponse<PrintResult>();
            try
            {
                var term = await context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefaultAsync();
                if (term == null)
                {
                    res.Message.FriendlyMessage = "Term not found";
                    return res;
                }

                var results = context.ScoreEntry
                   .Include(d => d.StudentContact).ThenInclude(d => d.User)
                   .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Session)
                    .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(s => s.GradeLevel).ThenInclude(x => x.Grades)
                   .Include(r => r.ClassScoreEntry).ThenInclude(s => s.Subject)
                   .Where(rr => rr.ClassScoreEntry.SessionClassId == sessionClassId && rr.SessionTermId == termId).AsEnumerable()
                   .GroupBy(d => d.StudentContactId, (Key, g) => new { studentId = Key, lst = g.ToList() })
                   .Select(s => new PrintResult(s.lst, regNoFormat, term, studentContactId))
                   .ToList() ?? new List<PrintResult>();


                if (results.Any())
                {
                    var averages = results.Select(d => d.average);
                    var studentPositions = UtilTools.GetStudentPositions(averages);
                    var studentResult = results.FirstOrDefault(d => d.studentContactId == studentContactId);
                    studentResult.position = studentPositions.FirstOrDefault(d => d.Average == studentResult.average)?.Position ?? "";
                    studentResult.noOfStudents = results.Count();
                    studentResult.isPublished = IsResultPublished(sessionClassId, termId, studentContactId);
                    res.Result = studentResult;
                }
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<PreviewResult>> IResultsService.GetStudentResultForPreviewAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var res = new APIResponse<PreviewResult>();
            var term = await context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefaultAsync();
            if (term == null)
            {
                res.Message.FriendlyMessage = "Term not found";
                return res;
            }

            var result = context.ScoreEntry
                .Include(d => d.StudentContact).ThenInclude(d => d.User)
                .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Session)
                .Include(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(f => f.GradeLevel).ThenInclude(s => s.Grades)
                .Include(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                .Where(rr => rr.ClassScoreEntry.SessionClassId == sessionClassId && rr.SessionTermId == termId 
                && rr.ClassScoreEntry.Subject.Deleted == false
                 && rr.ClassScoreEntry.Subject.IsActive == true).AsEnumerable().GroupBy(x => x.StudentContactId)
                .Select(g => new PreviewResult(g, regNoFormat, sessionClassId)).ToList() ?? new List<PreviewResult>();

            if (result.Any())
            {
                var averages = result.Select(d => d.average);
                var studentPositions = UtilTools.GetStudentPositions(averages);
                var studentResult = result.FirstOrDefault(d => d.studentContactId == studentContactId);
                if(studentResult != null)
                {
                    studentResult.noOfStudents = result.Count();
                    studentResult.position = studentPositions.FirstOrDefault(d => d.Average == studentResult.average)?.Position ?? "";
                    res.Result = studentResult;
                }
               

            }
            res.IsSuccessful = true;
            return res;
        }

        private bool IsResultPublished(Guid classId, Guid termId) =>
            context.SessionClassArchive.Any(d => d.SessionClassId == classId && termId == d.SessionTermId && d.IsPublished == true);

        private bool IsResultPublished(Guid classId, Guid termId, Guid studentId) =>
           context.SessionClassArchive.FirstOrDefault(d => d.SessionClassId == classId && termId == d.SessionTermId && d.StudentContactId == studentId)?.IsPublished ?? false;

        async Task<bool> IResultsService.AllResultPublishedAsync()
        {
            var currentTerm = await context.SessionTerm.FirstOrDefaultAsync(d => d.IsActive);
            var currentSessionClasses = await context.SessionClass.Include(x => x.Students).Where(d => d.Session.IsActive && d.Deleted == false && d.Students.Any()).ToListAsync();
            foreach(var currentSessionClass in currentSessionClasses)
            {
                if(!context.SessionClassArchive.Any(d => d.SessionClassId == currentSessionClass.SessionClassId))
                    return false;
            }
            return true;
        }

        async Task<APIResponse<BatchPrintDetail>> IResultsService.GetStudentsForBachPrinting(Guid sessionClassId, Guid termId)
        {
            var res = new APIResponse<BatchPrintDetail>();
            res.Result = new BatchPrintDetail();
            res.Result.Students = new List<StudentResultDetail>();
           
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            try
            {
                var clas = context.SessionClass.Include(x => x.Session).Include(x => x.Class).FirstOrDefault(d => d.SessionClassId == sessionClassId);
                var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();
                var pins = context.UploadedPin.Include(x => x.UsedPin).Where(d => d.Deleted == false && !d.UsedPin.Any()).AsEnumerable();

                if (clas.Session.IsActive)
                {
                    var result = await context.StudentContact
                       .Include(d => d.User)
                       .Include(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry)
                       .Where(rr => rr.SessionClassId == sessionClassId)
                       .Select(g => new StudentResultDetail(g, regNoFormat, sessionClassId, term.SessionTermId)).ToListAsync();

                    if (result != null)
                    {
                        var averages = result.Select(d => d.AverageScore);
                        var studentPositions = UtilTools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";
                        }
                        result = result.Where(x => x.AverageScore > 0).OrderByDescending(d => d.AverageScore).ToList();
                        res.Result.Students = result;
                    } 
                
                }
                else
                {
                    var classArchive = context.SessionClassArchive.Where(d => d.SessionClassId == sessionClassId && d.SessionTermId == termId).ToList();
                    if (!classArchive.Any())
                    {
                        res.Message.FriendlyMessage = "Result for this session and term was not published";
                        return res;
                    }

                    var result = context.ScoreEntry
                    .Include(e => e.ClassScoreEntry).ThenInclude(x => x.SessionClass)
                    .Include(d => d.StudentContact).ThenInclude(d => d.User)
                    .Where(rr => rr.ClassScoreEntry.SessionClassId == sessionClassId && termId == rr.SessionTermId && rr.IsOffered).AsEnumerable().GroupBy(s => s.StudentContactId)
                    .Select(entries => new StudentResultDetail(entries, regNoFormat)).ToList();

                    if (result != null)
                    {
                        var averages = result.Select(x => x.AverageScore);
                        var studentPositions = UtilTools.GetStudentPositions(averages);
                        foreach (var item in result)
                        {
                            item.Position = studentPositions.FirstOrDefault(d => d.Average == (decimal)item.AverageScore)?.Position ?? "";
                        }
                        res.Result.Students = result.Where(x => x.AverageScore > 0).ToList();
                    }
                }

                var pinstatus = "";
                var studentCount = res.Result.Students.Count();
                var pinCount = pins.Count();
                var numberOfPins = 0;
                if (studentCount > pinCount)
                {
                    pinstatus = "insufficient";
                    numberOfPins = pinCount;
                }
                else
                {
                    pinstatus = "sufficient";
                    numberOfPins = studentCount;
                } 
                var rs = new BatchPrintDetail(clas, term, numberOfPins, pinstatus, studentCount);
                rs.Students = res.Result.Students;
                res.Result = rs;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }


        }

        async Task<APIResponse<List<PublishList>>> IResultsService.GetPublishedList()
        {
            var res = new APIResponse<List<PublishList>>();
            res.Result = new List<PublishList>();
            var currentTerm = context.SessionTerm.FirstOrDefault(x => x.IsActive);
            var classes = await context.SessionClass.Include(x => x.Class).Include(x => x.Session).Where(x => x.Session.IsActive).Select(c => new { id = c.SessionClassId, name = c.Class.Name }).ToListAsync();
            foreach(var clas in classes)
            {
                var pubItem = new PublishList();
                pubItem.SessionClass = clas.name;
                pubItem.Status = IsResultPublished(clas.id, currentTerm.SessionTermId) ? "published" : "unpublished";
                res.Result.Add(pubItem);
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }


    }
}
