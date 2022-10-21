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
        Task<APIResponse<bool>> PromoteClassAsync(Promote request);
        Task<APIResponse<List<GetStudents>>> GetAllPassedStudentsAsync(FetchPassedOrFailedStudents request);
        Task<APIResponse<List<GetStudents>>> GetAllFailedStudentsAsync(FetchPassedOrFailedStudents request);
    }
}