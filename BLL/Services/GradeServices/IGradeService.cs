using BLL;
using Contracts.Common;
using SMP.Contracts.GradeModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.GradeServices
{
    public interface IGradeService
    {
        Task<APIResponse<AddGradeGroupModel>> CreateGradeAsync(AddGradeGroupModel request);
        Task<APIResponse<List<GetGradeGroupModel>>> GetGradeSettingAsync();
        Task<APIResponse<List<GetSessionClass>>> GetClassesAsync();
        Task<APIResponse<EditGradeGroupModel>> UpdateGradeAsync(EditGradeGroupModel request);
        Task<APIResponse<SingleDelete>> DeleteGradeAsync(SingleDelete request);
    }
}