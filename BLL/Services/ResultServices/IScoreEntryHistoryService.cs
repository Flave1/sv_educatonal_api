using DAL.StudentInformation;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public interface IScoreEntryHistoryService
    {
        ScoreEntryHistory GetScoreEntryHistory(string SessionClassId, string SubjectId, string CurrentTermId, string StudentContactId,  HistorySource source);
        Task<float> CreateNewScoreEntryHistoryAndReturnScore(ScoreEntryHistory scoreHistory, float resultScore, string studentContactId, string sessionClassId, string subjectId, Guid termId, bool Include, HistorySource source);
        float IncludeAndExcludeThenReturnScore(ScoreEntryHistory scoreHistory, bool Include, float resultScore);
        ScoreEntry GetScoreEntry(Guid CurrentTermId, Guid StudentContactId, Guid SubjectId);
        void CreateNewScoreEntryForAssessment(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId);
        void UpdateScoreEntryForAssessment(ScoreEntry scoreEntry, float ResultScore);
        void UpdateScoreEntryForExam(ScoreEntry scoreEntry, float ResultScore);
        void CreateNewScoreEntryForExam(ScoreEntry scoreEntry, Guid CurrentTermId, float ResultScore, Guid StudentContactId, Guid SubjectId, Guid SessionClassId);
        List<MasterListResult> GetMasterListFromScoreEntry(Guid sessionClassId, Guid termId, string regNoFormat);
        List<CumulativeMasterListResult> GetCumulativeMasterListFromScoreEntry(Guid sessionClassId, string regNoFormat);
        IQueryable<StudentContact> GetClassStudentInQuery(Guid sessionClassId);
        IQueryable<IGrouping<Guid, ScoreEntry>> GetResultFromScoreEntryQuery(Guid sessionClassId, Guid termId);
        IQueryable<StudentSessionClassHistory> GetStudentsFromArchiveQuery(Guid sessionClassId, Guid sessionTermId);
        Task<ScoreEntry> CreateScoreEntryForExam(UpdateScore request);
        Task<ScoreEntry> UpdateScoreEntryForExam(UpdateScore request, ScoreEntry studentEntry);
        Task<ScoreEntry> CreateScoreEntryForAssessment(UpdateScore request);
        Task<ScoreEntry> UpdateScoreEntryForAssessment(UpdateScore request, ScoreEntry studentEntry);
        IQueryable<ScoreEntry> GetScoreEntriesQuery(Guid subjectId, Guid sessionClassId, Guid sessionTermId);
    }
}