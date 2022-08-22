using BLL;
using SMP.Contracts.Assessment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AssessmentServices
{
    public interface IClassAssessmentService
    {
        Task<APIResponse<GetClassAssessmentRequest>> CreateClassAssessmentAsync(CreateClassAssessment request);
        Task<APIResponse<List<ClassAssessmentStudents>>> GetClassStudentByAssessmentAsync(Guid classAssessmentId);
        Task<APIResponse<UpdatetudentAssessmentScore>> UpdateStudentAssessmentScoreAsync(UpdatetudentAssessmentScore request);
        Task<APIResponse<List<GetClassAssessmentRequest>>> GetAssessmentByTeacherAsync();
        Task<APIResponse<UpdatClassAssessmentScore>> UpdateClassAssessmentScoreAsync(UpdatClassAssessmentScore request);
    }
}