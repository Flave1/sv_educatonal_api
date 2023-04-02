using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Utilities;
using SMP.Contracts.ResultModels;
using SMP.DAL.Migrations;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public class ScoreEntryHistoryService : IScoreEntryHistoryService
    {
        private readonly DataContext context;
        private readonly string smsClientId;

        public ScoreEntryHistoryService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        DAL.Models.ResultModels.ScoreEntryHistory IScoreEntryHistoryService.GetScoreEntryHistory(string SessionClassId, string SubjectId, string CurrentTermId, string StudentContactId, HistorySource source) =>
            context.ScoreEntryHistory.FirstOrDefault(x => x.SessionClassId == SessionClassId && x.Subjectid == SubjectId.ToString()
                    && x.SessionTermId == CurrentTermId && x.StudentId == StudentContactId && x.ClientId == smsClientId && (int)source == x.Source);

        ScoreEntry IScoreEntryHistoryService.GetScoreEntry(Guid CurrentTermId, Guid StudentContactId, Guid SubjectId) => 
            context.ScoreEntry.FirstOrDefault(s => s.SessionTermId == CurrentTermId && s.StudentContactId == StudentContactId && s.SubjectId == SubjectId && s.ClientId == smsClientId);
        
        async Task<float> IScoreEntryHistoryService.CreateNewScoreEntryHistoryAndReturnScore(DAL.Models.ResultModels.ScoreEntryHistory scoreHistory, float resultScore, string studentContactId, string sessionClassId, string subjectId, Guid termId, bool Include, HistorySource source)
        {
            scoreHistory = new DAL.Models.ResultModels.ScoreEntryHistory();
            scoreHistory.StudentId = studentContactId;
            scoreHistory.SessionClassId = sessionClassId;
            scoreHistory.Subjectid = subjectId;
            scoreHistory.SessionTermId = termId.ToString();
            scoreHistory.Score = scoreHistory.Score + '|' + resultScore;
            scoreHistory.Source = (int)source;
            context.ScoreEntryHistory.Add(scoreHistory);
            await context.SaveChangesAsync();
            return +resultScore;
        }

        float IScoreEntryHistoryService.IncludeAndExcludeThenReturnScore(DAL.Models.ResultModels.ScoreEntryHistory scoreHistory, bool Include, float resultScore)
        {
            if (Include)
            {
                List<string> previousScores = scoreHistory.Score.Split('|').ToList();
                if(previousScores.Any())
                {
                    previousScores.Add(resultScore.ToString());
                    scoreHistory.Score = string.Join('|', previousScores);
                }else
                    scoreHistory.Score = scoreHistory.Score + '|' + resultScore;
                context.SaveChanges();
                return +resultScore;
            }
            else
            {
                var list = scoreHistory.Score.Split('|').ToList();
                var filtered = list.Remove(resultScore.ToString());
                scoreHistory.Score = string.Join('|', list);
                context.SaveChanges();
                return -resultScore;
            }
        }

        void IScoreEntryHistoryService.CreateNewScoreEntryForAssessment(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId)
        {
            scoreEntry = new ScoreEntry();
            scoreEntry.SessionTermId = CurrentTermId;
            scoreEntry.AssessmentScore = (int)ResultScore;
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
            scoreEntry.SubjectId = SubjectId;
            scoreEntry.SessionClassId = SessionClassId;
            scoreEntry.StudentContactId = StudentContactId;
            context.ScoreEntry.Add(scoreEntry);
        }

        void IScoreEntryHistoryService.UpdateScoreEntryForAssessment(ScoreEntry scoreEntry, float ResultScore)
        {
            scoreEntry.AssessmentScore = (scoreEntry.AssessmentScore+(int)ResultScore);
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
        }

        void IScoreEntryHistoryService.CreateNewScoreEntryForExam(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId)
        {
            scoreEntry = new ScoreEntry();
            scoreEntry.SessionTermId = CurrentTermId;
            scoreEntry.ExamScore = (int)ResultScore;
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
            scoreEntry.SubjectId = SubjectId;
            scoreEntry.SessionClassId = SessionClassId;
            scoreEntry.StudentContactId = StudentContactId;
            context.ScoreEntry.Add(scoreEntry);
        }

        void IScoreEntryHistoryService.UpdateScoreEntryForExam(ScoreEntry scoreEntry, float ResultScore)
        {
            scoreEntry.ExamScore = (scoreEntry.ExamScore + (int)ResultScore);
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
        }

        List<MasterListResult> IScoreEntryHistoryService.GetMasterListFromScoreEntry(Guid sessionClassId, Guid termId, string regNoFormat)
        {
            return context.ScoreEntry
                     .Where(x => x.ClientId == smsClientId)
                     .Include(d => d.Subject)
                     .Include(r => r.StudentContact)
                     .Include(x => x.SessionClass).ThenInclude(x => x.Class)
                     .Where(r => r.SessionClassId == sessionClassId && r.SessionTermId == termId).AsEnumerable().GroupBy(x => x.StudentContactId)
                     .Select(g => new MasterListResult(g, regNoFormat)).ToList();
        }

        List<CumulativeMasterListResult> IScoreEntryHistoryService.GetCumulativeMasterListFromScoreEntry(Guid sessionClassId, string regNoFormat)
        {
            return context.ScoreEntry.Where(x => x.ClientId == smsClientId)
                     .Include(s => s.SessionTerm)
                     .Include(r => r.StudentContact)
                     .Include(r => r.SessionClass).Include(d => d.Subject)
                     .Where(r => r.SessionClassId == sessionClassId && r.Subject.Deleted == false).AsEnumerable().GroupBy(s => s.StudentContactId)
                     .Select(g => new CumulativeMasterListResult(g, regNoFormat)).ToList();
        }
        IQueryable<StudentContact> IScoreEntryHistoryService.GetClassStudentInQuery(Guid sessionClassId)
        {
            return context.StudentContact
                       .Where(rr => rr.ClientId == smsClientId && rr.SessionClassId == sessionClassId)
                       .Include(d => d.User)
                       .AsQueryable();
        }

        IQueryable<IGrouping<Guid, ScoreEntry>> IScoreEntryHistoryService.GetResultFromScoreEntryQuery(Guid sessionClassId, Guid termId)
        {
            return context.ScoreEntry
                        .Where(rr => rr.ClientId == smsClientId && rr.SessionClassId == sessionClassId && termId == rr.SessionTermId)
                        .Include(x => x.SessionClass)
                        .Include(d => d.StudentContact).AsEnumerable().GroupBy(s => s.StudentContactId).AsQueryable();
        }

        IQueryable<StudentSessionClassHistory> IScoreEntryHistoryService.GetStudentsFromArchiveQuery(Guid sessionClassId, Guid sessionTermId)
        {
            return context.StudentSessionClassHistory.Where(d => d.ClientId == smsClientId && d.SessionClassId == sessionClassId && sessionTermId == d.SessionTermId);
        }
        IQueryable<ScoreEntry> IScoreEntryHistoryService.GetScoreEntriesQuery(Guid subjectId, Guid sessionClassId, Guid sessionTermId) =>
           context.ScoreEntry.Where(x => x.ClientId == smsClientId && x.SubjectId == subjectId && x.SessionClassId == sessionClassId && x.SessionTermId == sessionTermId);
        async Task<ScoreEntry> IScoreEntryHistoryService.CreateScoreEntryForExam(UpdateScore request)
        {
            ScoreEntry studentEntry = new ScoreEntry();
            studentEntry.ExamScore = request.Score;
            studentEntry.IsSaved = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.IsOffered = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.SessionTermId = Guid.Parse(request.SessionTermId);
            studentEntry.StudentContactId = Guid.Parse(request.StudentContactId);
            studentEntry.SubjectId = Guid.Parse(request.SubjectId);
            studentEntry.SessionClassId = Guid.Parse(request.SessionClassId);
            await context.AddAsync(studentEntry);
            await context.SaveChangesAsync();
            return studentEntry;
        }

        async Task<ScoreEntry> IScoreEntryHistoryService.UpdateScoreEntryForExam(UpdateScore request, ScoreEntry studentEntry)
        {
            studentEntry.ExamScore = request.Score;
            studentEntry.IsSaved = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.IsOffered = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            await context.SaveChangesAsync();
            return studentEntry;
        }

        async Task<ScoreEntry> IScoreEntryHistoryService.CreateScoreEntryForAssessment(UpdateScore request)
        {
            ScoreEntry studentEntry = new ScoreEntry();
            studentEntry.AssessmentScore = request.Score;
            studentEntry.IsSaved = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.IsOffered = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.SessionTermId = Guid.Parse(request.SessionTermId);
            studentEntry.StudentContactId = Guid.Parse(request.StudentContactId);
            studentEntry.SubjectId = Guid.Parse(request.SubjectId);
            studentEntry.SessionClassId = Guid.Parse(request.SessionClassId);
            await context.AddAsync(studentEntry);
            await context.SaveChangesAsync();
            return studentEntry;
        }

        async Task<ScoreEntry> IScoreEntryHistoryService.UpdateScoreEntryForAssessment(UpdateScore request, ScoreEntry studentEntry)
        {
            studentEntry.AssessmentScore = request.Score;
            studentEntry.IsSaved = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            studentEntry.IsOffered = studentEntry.ExamScore > 0 || studentEntry.AssessmentScore > 0;
            await context.SaveChangesAsync();
            return studentEntry;
        }
    }
}
