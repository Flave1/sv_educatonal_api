using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using Microsoft.AspNetCore.Http;
using SMP.Contracts.Assessment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AssessmentServices
{
    public interface IHomeAssessmentService
    {
        Task<APIResponse<CreateHomeAssessmentRequest>> CreateHomeAssessmentAsync(CreateHomeAssessmentRequest request);
        Task<APIResponse<UpdateHomeAssessmentRequest>> UpdateHomeAssessmentAsync(UpdateHomeAssessmentRequest request);
        Task<APIResponse<PagedResponse<List<GetHomeAssessmentRequest>>>> GetSubjectHomeAssessmentAsync(string classId, string sessionClassSubjectId, string groupId, PaginationFilter filter);
        Task<APIResponse<bool>> DeleteHomeAssessmentAsync(SingleDelete request);
        Task<APIResponse<bool>> SendHomeAssessmentToStudentsAsync(SendHomeAssessmentRequest request);
        Task<APIResponse<GetHomeAssessmentRequest>> GetSingleHomeAssessmentAsync(Guid homeAssessmentId, string sessionClasId);
        Task<APIResponse<GetClassAssessmentRecord>> GetSubjectAssessmentScoreRecordAsync(Guid sessionClassSubjectId, Guid sessionClasId);
        Task<APIResponse<List<StudentHomeAssessmentRequest>>> GetHomeAssessmentsByStudentAsync();

        Task<APIResponse<CreateHomeAssessmentFeedback>> SubmitHomeAssessmentByStudentAsync(CreateHomeAssessmentFeedback request);
        Task<APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>> FilterHomeAssessmentsByStudentAsync(int status, PaginationFilter filter);
        Task<APIResponse<GetHomeAssessmentFeedback>> GetSingleHomeAssessmentsByStudentAsync(Guid homeAssessmentFeedBackId);
        Task<APIResponse<ScoreHomeAssessmentFeedback>> ScoreHomeAssessmentByStudentAsync(ScoreHomeAssessmentFeedback request);
        Task<APIResponse<GetHomeAssessmentFeedback>> GetSingleHomeAssessmentsByTeacherAsync(Guid homeAssessmentFeedBackId);
        Task<APIResponse<bool>> CloseHomeAssessmentAsync(string homeAssessmentId);
        Task<APIResponse<List<SubmittedAndUnsubmittedStudents>>> GetHomeAssessmentRecord(string homeAssessmentId);
        Task<APIResponse<bool>> IncludeClassAssessmentToScoreEntry(string homeAssessmentId, bool Include);
        Task<APIResponse<bool>> IncludeStudentAssessmentToScoreEntry(string homeAssessmentFeedbackId, bool include);
        APIResponse<string> ReadFileContent(IFormFile file);
        Task<APIResponse<PagedResponse<List<StudentHomeAssessmentRequest>>>> FilterHomeAssessmentsByParentAsync(Guid sessionClassSubjectId, string studentContactid, PaginationFilter filter);
    }
}