using BLL;
using DAL.ClassEntities;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public interface IResultsService
    {
        Task<APIResponse<List<GetClasses>>> GetCurrentStaffClassesAsync();
        Task<APIResponse<List<GetClassSubjects>>> GetCurrentStaffClassSubjectsAsync(Guid sessionClassId);
        Task<APIResponse<GetClassScoreEntry>> GetClassEntryAsync(Guid sessionClassId, Guid subjectId);
        Task CreateClassScoreEntryAsync(SessionClass sessionClass);
        Task<APIResponse<ScoreEntry>> UpdateExamScore(UpdateScore request);
        Task<APIResponse<ScoreEntry>> UpdateAssessmentScore(UpdateScore request);
        Task<APIResponse<PreviewClassScoreEntry>> PreviewClassScoreEntry(Guid sessionClassId, Guid subjectId);
        Task<APIResponse<MasterList>> GetMasterListAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<StudentResult>> GetListOfResultsAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<PublishResultRequest>> PublishResultAsync(PublishResultRequest request);
        Task<APIResponse<GetClassScoreEntry>> GetPreviousTermsClassSubjectScoreEntriesAsync(Guid sessionClassId, Guid subjectId, Guid sessionTermId);
        Task<APIResponse<ScoreEntry>> UpdatePreviousTermsExamScore(UpdateOtherSessionScore request);
        Task<APIResponse<ScoreEntry>> UpdatePreviousTermsAssessmentScore(UpdateOtherSessionScore request);
        Task<APIResponse<PreviewClassScoreEntry>> PreviewPreviousTermsClassScoreEntry(Guid sessionClassId, Guid subjectId, Guid sessionTermId);
        Task<APIResponse<CumulativeMasterList>> GetCumulativeMasterListAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<StudentCoreEntry>> GetSingleStudentScoreEntryAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
        Task<APIResponse<PreviewResult>> GetStudentResultAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
    }
}