﻿using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
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
        Task<APIResponse<UpdateStudentAssessmentScore>> UpdateStudentAssessmentScoreAsync(UpdateStudentAssessmentScore request);
        Task<APIResponse<PagedResponse<List<GetClassAssessmentRequest>>>> GetAssessmentByTeacherAsync(string sessionClassId, string sessionClassSubjectId, PaginationFilter filter);
        Task<APIResponse<UpdatClassAssessmentScore>> UpdateClassAssessmentScoreAsync(UpdatClassAssessmentScore request);
        Task<APIResponse<GetClassAssessmentRequest>> GetSingleAssessmentAsync(Guid classAssessmentId);
        Task<APIResponse<SingleDelete>> DeleteClassAssessmentAsync(SingleDelete request);
    }
}