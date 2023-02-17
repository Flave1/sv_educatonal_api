using SMP.DAL.Models.ResultModels;
using System;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public interface IScoreEntryHistoryService
    {
        ScoreEntryHistory GetScoreEntryHistory(string SessionClassId, string SubjectId, string CurrentTermId, string StudentContactId);
        Task<float> CreateNewScoreEntryHistoryAndReturnScore(ScoreEntryHistory scoreHistory, float resultScore, string studentContactId, string sessionClassId, string subjectId, Guid termId, bool Include);
        float IncludeAndExcludeThenReturnScore(ScoreEntryHistory scoreHistory, bool Include, float resultScore);
        ScoreEntry GetScoreEntry(Guid CurrentTermId, Guid StudentContactId, Guid SubjectId);
        void CreateNewScoreEntryForAssessment(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId);
        void UpdateScoreEntryForAssessment(ScoreEntry scoreEntry, float ResultScore);
        void UpdateScoreEntryForExam(ScoreEntry scoreEntry, float ResultScore);
        void CreateNewScoreEntryForExam(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId);
    }
}