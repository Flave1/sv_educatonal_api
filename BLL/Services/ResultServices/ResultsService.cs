using BLL;
using BLL.Constants;
using BLL.Utilities;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
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
            var res = new APIResponse<List<GetClassSubjects>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    res.Result = await context.SessionClassSubject
                        .Include(d => d.Subject)
                        .Where(e => e.SessionClassId == sessionClassId).Select(s => new GetClassSubjects(s)).ToListAsync();

                }

                res.Result = await context.SessionClassSubject
                     .Include(d => d.Subject)
                     .Where(e => e.SessionClassId == sessionClassId).Select(s => new GetClassSubjects(s)).ToListAsync();
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassScoreEntry>> IResultsService.GetClassEntryAsync(Guid sessionClassId, Guid subjectId)
        {
            var res = new APIResponse<GetClassScoreEntry>();
            var currentTerm = await context.SessionTerm.FirstOrDefaultAsync(d => d.IsActive);
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            res.Result = await context.ClassScoreEntry
                .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                .Include(d => d.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                .Include(d => d.Subject)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new GetClassScoreEntry(s, regNoFormat, currentTerm)).FirstOrDefaultAsync();

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
                var currentTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
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
                var currentTerm = context.SessionTerm.FirstOrDefault(d => d.IsActive);
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

           var result = await context.SessionClass
                    .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                    .Include(r => r.Students).ThenInclude(d => d.User)
                    .Include(r => r.Session)
                    .Include(r => r.Class)
                    .Include(r => r.Teacher).ThenInclude(r => r.User)
                    .Where(r => r.SessionClassId == sessionClassId)
                    .Select(g => new MasterList(g, term, regNoFormat)).FirstOrDefaultAsync();

            if (result.ResultList != null)
            {
                var averages = result.ResultList.Select(d => d.AverageScore);
                var studentPositions = UtilTools.GetStudentPositions(averages);
                foreach(var item in result.ResultList)
                {
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position?? "";
                }

                result.ResultList = result.ResultList.OrderBy(d => d.Position).ToList();

            }
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<StudentResult>> IResultsService.GetListOfResultsAsync(Guid sessionClassId, Guid termId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
        
            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();
            var res = new APIResponse<StudentResult>();
            var result = await context.SessionClass
                .Include(e => e.PublishStatus)
                .Include(d => d.SessionClassSubjects).ThenInclude(d => d.Subject)
                .Include(d => d.Students).ThenInclude(d => d.User)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(r => r.ClassScoreEntries).ThenInclude(r => r.ScoreEntries)
                .Include(d => d.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass)
                .Where(rr => rr.SessionClassId == sessionClassId).Select(s => s.Students).Select(g => new StudentResult(g, regNoFormat, sessionClassId, term.SessionTermId)).FirstOrDefaultAsync();
        
            if (result != null)
            {
                result.IsPublished = context.PublishStatus.FirstOrDefault(d => d.SessionClassId == sessionClassId && d.SessionTermId == termId)?.IsPublished ?? false;
                var averages = result.PublishResult.Select(d => d.AverageScore);
                var studentPositions = UtilTools.GetStudentPositions(averages);
                foreach (var item in result.PublishResult)
                {
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == (decimal)item.AverageScore)?.Position ?? "";
                }

                result.PublishResult = result.PublishResult.OrderBy(d => d.Position).ToList();
            }
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<PublishResultRequest>> IResultsService.PublishResultAsync(PublishResultRequest request)
        {
            var res = new APIResponse<PublishResultRequest>();
            try
            {
                var sessClass = await context.SessionClass.Include(e => e.PublishStatus).FirstOrDefaultAsync(d => d.SessionClassId == request.SessionClassId && d.PublishStatus.SessionTermId == request.SessionTermId);
                if (sessClass != null)
                {
                    sessClass.PublishStatus.IsPublished = request.Publish;
                    await context.SaveChangesAsync();
                }
                else
                {
                    sessClass = await context.SessionClass.FindAsync(request.SessionClassId);
                    sessClass.PublishStatus = new PublishStatus();
                    sessClass.PublishStatus.IsPublished = request.Publish;
                    sessClass.PublishStatus.SessionClassId = request.SessionClassId;
                    sessClass.PublishStatus.SessionTermId = request.SessionTermId;
                    await context.SaveChangesAsync();
                }
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successful";
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
            var currentTerm = await context.SessionTerm.FirstOrDefaultAsync(d => d.SessionTermId == sessionTermId);
            if(currentTerm != null)
            {
                var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
                res.Result = await context.ClassScoreEntry
                    .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                    .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                    .Include(d => d.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                    .Include(d => d.Subject)
                    .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                    .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new GetClassScoreEntry(s, regNoFormat, currentTerm)).FirstOrDefaultAsync();
            }
            else
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
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

            var result = await context.SessionClass
                     .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                     .Include(r => r.Students).ThenInclude(d => d.User)
                     .Include(r => r.Session).ThenInclude(d => d.Terms)
                     .Include(r => r.Class)
                     .Include(r => r.Teacher).ThenInclude(r => r.User)
                     .Where(r => r.SessionClassId == sessionClassId)
                     .Select(g => new CumulativeMasterList(g, regNoFormat)).FirstOrDefaultAsync();

            if (result.ResultList != null)
            {
                var averages = result.ResultList.Select(d => d.AverageScore);
                var studentPositions = UtilTools.GetStudentPositions(averages);

                foreach (var item in result.ResultList)
                    item.Position = studentPositions.FirstOrDefault(d => d.Average == item.AverageScore)?.Position ?? "";

                result.ResultList = result.ResultList.OrderBy(d => d.Position).ToList();

            }
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<StudentCoreEntry>> IResultsService.GetSingleStudentScoreEntryAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();

            var res = new APIResponse<StudentCoreEntry>();

            var result = await context.SessionClass
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.Subject)
                 .Include(r => r.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(x => x.SessionTerm)
                 .Include(r => r.Students).ThenInclude(d => d.User)
                 .Include(r => r.Students).ThenInclude(d => d.SessionClass).ThenInclude(d => d.PublishStatus)
                 .Include(r => r.Students).ThenInclude(d => d.SessionClass).ThenInclude(d => d.Class).ThenInclude(d => d.GradeLevel).ThenInclude(d => d.Grades)
                 .Where(r => r.SessionClassId == sessionClassId)
                 .Select(g => new StudentCoreEntry(g.Students.FirstOrDefault(x => x.StudentContactId == studentContactId), regNoFormat, termId)).FirstOrDefaultAsync();

            if (result != null)
                result.IsPublished = context.PublishStatus.FirstOrDefault(d => d.SessionClassId == sessionClassId && d.SessionTermId == termId)?.IsPublished ?? false;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<PreviewResult>> IResultsService.GetStudentResultAsync(Guid sessionClassId, Guid termId, Guid studentContactId)
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var term = context.SessionTerm.Where(e => e.SessionTermId == termId).FirstOrDefault();
            var res = new APIResponse<PreviewResult>();
            var result = await context.SessionClass
                .Include(e => e.PublishStatus)
                .Include(e => e.Class).ThenInclude(d => d.GradeLevel)
                .Include(d => d.SessionClassSubjects).ThenInclude(d => d.Subject)
                .Include(d => d.Students).ThenInclude(d => d.User)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Session)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(e => e.Class).ThenInclude(f=>f.GradeLevel).ThenInclude(s=>s.Grades)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(e => e.PublishStatus)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(r => r.ClassScoreEntries).ThenInclude(r => r.ScoreEntries)
                .Include(d => d.Students).ThenInclude(d => d.SessionClass).ThenInclude(r => r.ClassScoreEntries).ThenInclude(s => s.Subject)
                .Include(d => d.Students).ThenInclude(d => d.ScoreEntries).ThenInclude(d => d.ClassScoreEntry).ThenInclude(d => d.SessionClass)
                .Where(rr => rr.SessionClassId == sessionClassId).Select(s => s.Students.Where(d => d.EnrollmentStatus == 1)).SelectMany(s => s).Select(g => new PreviewResult(g, regNoFormat, sessionClassId, term.SessionTermId, studentContactId)).ToListAsync() ?? new List<PreviewResult>();

            if (result.Any())
            {
                var averages = result.Select(d => d.average);
                var studentPositions = UtilTools.GetStudentPositions(averages);
                var studentResult = result.FirstOrDefault(d => d.studentContactId == studentContactId);
                studentResult.position = studentPositions.FirstOrDefault(d => d.Average == studentResult.average)?.Position ?? "";
                res.Result = studentResult;
            }
            res.IsSuccessful = true;
            return res;
        }
    }
}
