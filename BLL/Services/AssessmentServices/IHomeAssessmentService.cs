using BLL;
using Contracts.Common;
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
        Task<APIResponse<List<GetHomeAssessmentRequest>>> GetSubjectHomeAssessmentAsync(string classId, string sessionClassSubjectId, string groupId);
        Task<APIResponse<bool>> DeleteHomeAssessmentAsync(SingleDelete request);
        Task<APIResponse<bool>> SendHomeAssessmentToStudentsAsync(SendHomeAssessmentRequest request);
        Task<APIResponse<GetHomeAssessmentRequest>> GetSingleHomeAssessmentAsync(Guid homeAssessmentId, string sessionClasId);
        Task<APIResponse<GetClassAssessmentRecord>> GetSubjectAssessmentScoreRecordAsync(Guid sessionClassSubjectId, Guid sessionClasId);
        Task<APIResponse<List<StudentHomeAssessmentRequest>>> GetHomeAssessmentsByStudentAsync();

        Task<APIResponse<CreateHomeAssessmentFeedback>> SubmitHomeAssessmentByStudentAsync(CreateHomeAssessmentFeedback request);
        Task<APIResponse<List<StudentHomeAssessmentRequest>>> FilterHomeAssessmentsByStudentAsync(int status);
        Task<APIResponse<GetHomeAssessmentFeedback>> GetSingleHomeAssessmentsByStudentAsync(Guid homeAssessmentFeedBackId);
        Task<APIResponse<ScoreHomeAssessmentFeedback>> ScoreHomeAssessmentByStudentAsync(ScoreHomeAssessmentFeedback request);
        Task<APIResponse<GetHomeAssessmentFeedback>> GetSingleHomeAssessmentsByTeacherAsync(Guid homeAssessmentFeedBackId);
        Task<APIResponse<bool>> CloseHomeAssessmentAsync(string homeAssessmentId);
        Task<APIResponse<List<SubmittedAndUnsubmittedStudents>>> GetHomeAssessmentRecord(string homeAssessmentId);
        Task<APIResponse<bool>> IncludeClassAssessmentToScoreEntry(string homeAssessmentId);
        Task<APIResponse<bool>> IncludeStudentAssessmentToScoreEntry(string homeAssessmentFeedbackId);
    }
}