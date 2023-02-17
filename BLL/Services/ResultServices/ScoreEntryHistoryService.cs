using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using DAL;
using DAL.ClassEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Utilities;
using SMP.Contracts.ResultModels;
using SMP.DAL.Migrations;
using SMP.DAL.Models.ResultModels;
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

        DAL.Models.ResultModels.ScoreEntryHistory IScoreEntryHistoryService.GetScoreEntryHistory(string SessionClassId, string SubjectId, string CurrentTermId, string StudentContactId) =>
            context.ScoreEntryHistory.FirstOrDefault(x => x.SessionClassId == SessionClassId && x.Subjectid == SubjectId.ToString()
                    && x.SessionTermId == CurrentTermId && x.StudentId == StudentContactId && x.ClientId == smsClientId);

        ScoreEntry IScoreEntryHistoryService.GetScoreEntry(Guid CurrentTermId, Guid StudentContactId, Guid SubjectId) => context.ScoreEntry.FirstOrDefault(s => s.SessionTermId == CurrentTermId && s.StudentContactId == StudentContactId && s.ClassScoreEntry.SubjectId == SubjectId && s.ClientId == smsClientId);
        
        async Task<float> IScoreEntryHistoryService.CreateNewScoreEntryHistoryAndReturnScore(DAL.Models.ResultModels.ScoreEntryHistory scoreHistory, float resultScore, string studentContactId, string sessionClassId, string subjectId, Guid termId, bool Include)
        {
            scoreHistory = new DAL.Models.ResultModels.ScoreEntryHistory();
            scoreHistory.StudentId = studentContactId;
            scoreHistory.SessionClassId = sessionClassId;
            scoreHistory.Subjectid = subjectId;
            scoreHistory.SessionTermId = termId.ToString();
            scoreHistory.Score = scoreHistory.Score + '|' + resultScore;

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
            scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == SubjectId && x.SessionClassId == SessionClassId && x.ClientId == smsClientId).ClassScoreEntryId;
            scoreEntry.AssessmentScore = (int)ResultScore;
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
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
            scoreEntry.ClassScoreEntryId = context.ClassScoreEntry.FirstOrDefault(x => x.SubjectId == SubjectId && x.SessionClassId == SessionClassId && x.ClientId == smsClientId).ClassScoreEntryId;
            scoreEntry.ExamScore = (int)ResultScore;
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
            scoreEntry.StudentContactId = StudentContactId;
            context.ScoreEntry.Add(scoreEntry);
        }

        void IScoreEntryHistoryService.UpdateScoreEntryForExam(ScoreEntry scoreEntry, float ResultScore)
        {
            scoreEntry.ExamScore = (scoreEntry.ExamScore + (int)ResultScore);
            scoreEntry.IsOffered = true;
            scoreEntry.IsSaved = true;
        }
    }
}
