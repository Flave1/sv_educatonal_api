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
        Task<APIResponse<List<GetHomeAssessmentRequest>>> GetSubjectHomeAssessmentAsync(Guid SessionClassSubjectId);
        Task<APIResponse<bool>> DeleteHomeAssessmentAsync(SingleDelete request);
        Task<APIResponse<bool>> SendHomeAssessmentToStudentsAsync(SendHomeAssessmentRequest request);
        Task<APIResponse<GetHomeAssessmentRequest>> GetSingleHomeAssessmentAsync(Guid homeAssessmentId, string sessionClasId);
        Task<APIResponse<GetClassAssessmentRecord>> GetSubjectAssessmentScoreRecordAsync(Guid sessionClassSubjectId, Guid sessionClasId);
        Task<APIResponse<List<StudentHomeAssessmentRequest>>> GetHomeAssessmentsByStudentAsync();

        Task<APIResponse<CreateHomeAssessmentFeedback>> SubmitHomeAssessmentByStudentAsync(CreateHomeAssessmentFeedback request);
        Task<APIResponse<List<StudentHomeAssessmentRequest>>> FilterHomeAssessmentsByStudentAsync(int status);
        Task<APIResponse<GetHomeAssessmentFeedback>> GetSingleHomeAssessmentsByStudentAsync(Guid homeAssessmentFeedBackId);
        Task<APIResponse<ScoreHomeAssessmentFeedback>> ScoreHomeAssessmentByStudentAsync(ScoreHomeAssessmentFeedback request);
    }
}