using BLL;
using SMP.Contracts.PromotionModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PromorionServices
{
    public interface IPromotionService
    {
        Task<APIResponse<List<PreviousSessionClasses>>> GetPreviousSessionClassesAsync();
        Task<APIResponse<bool>> PromoteClassAsync(Guid classToPromote, Guid classToPromoteTo);
        Task<APIResponse<List<GetStudents>>> GetAllPassedStudentsAsync(Guid SessionClassId);
        Task<APIResponse<List<GetStudents>>> GetAllFailedStudentsAsync(Guid SessionClassId);
    }
}