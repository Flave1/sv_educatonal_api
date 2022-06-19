using BLL;
using BLL.Constants;
using BLL.Utilities;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
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
            var res = new APIResponse<List<GetClasses>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    res.Result = await context.SessionClass
                        .Include(s => s.Class) 
                        .OrderBy(s => s.Class.Name)
                        .Include(s => s.Session)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false).Select(s => new GetClasses(s)).ToListAsync();
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
            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetClassScoreEntry>> IResultsService.GetClassEntryAsync(Guid sessionClassId, Guid subjectId)
        {
            var res = new APIResponse<GetClassScoreEntry>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            res.Result = await context.ClassScoreEntry
                .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(e => e.User)
                .Include(d => d.SessionClass).ThenInclude(d => d.Class)
                .Include(d => d.Subject)
                .Include(d => d.ScoreEntries).ThenInclude(s => s.StudentContact).ThenInclude(d => d.User)
                .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new GetClassScoreEntry(s, regNoFormat)).FirstOrDefaultAsync();

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
                .Where(e => e.SessionClassId == sessionClassId && e.SubjectId == subjectId).Select(s => new PreviewClassScoreEntry(s, regNoFormat)).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task IResultsService.CreateClassScoreEntryAsync(SessionClass sessionClass)
        {
            try
            {
                foreach (var subject in sessionClass.SessionClassSubjects)
                {
                    var classEntry = new ClassScoreEntry
                    {
                        SessionClassId = sessionClass.SessionClassId,
                        SubjectId = subject.SubjectId,
                    };
                    await context.ClassScoreEntry.AddAsync(classEntry);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task IResultsService.CreateClassScoreSubjectEntriesAsync(Guid studentId)
        {
            try
            {
                var student = await context.StudentContact.Include(d => d.SessionClass)
                    .ThenInclude(d => d.ClassScoreEntries).ThenInclude(s => s.ScoreEntries).FirstOrDefaultAsync(d => d.StudentContactId == studentId);
                if(student != null)
                {
                    foreach (var classEntry in student.SessionClass.ClassScoreEntries)
                    {
                        var scoreEntry = new ScoreEntry();
                        scoreEntry.AssessmentScore = 0;
                        scoreEntry.ClassScoreEntryId = classEntry.ClassScoreEntryId;
                        scoreEntry.ExamScore = 0;
                        scoreEntry.StudentContactId = student.StudentContactId;
                        await context.ScoreEntry.AddAsync(scoreEntry);
                        await context.SaveChangesAsync();
                    }
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
                var entry = await context.ScoreEntry.FindAsync(Guid.Parse(request.ScoreEntryId));
                if(entry != null)
                {
                    entry.ExamScore = request.Score;
                    entry.IsSaved = request.Score > 0 || entry.AssessmentScore > 0;
                    entry.IsOffered = request.Score > 0 || entry.AssessmentScore > 0;
                    await context.SaveChangesAsync();

                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                {
                    res.Message.FriendlyMessage = "Entry not found";
                }
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
                var entry = await context.ScoreEntry.FindAsync(Guid.Parse(request.ScoreEntryId));
                if (entry != null)
                {
                    entry.AssessmentScore = request.Score;
                    entry.IsSaved = request.Score > 0 || entry.AssessmentScore > 0;
                    entry.IsOffered = request.Score > 0 || entry.AssessmentScore > 0;
                    await context.SaveChangesAsync();

                    res.Result = entry;
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                {
                    res.Message.FriendlyMessage = "Entry not found";
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = ex?.Message;
                return res;
            }
        }
    }
}
