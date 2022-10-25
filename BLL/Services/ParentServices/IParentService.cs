using BLL;
using SMP.Contracts.Parents;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public interface IParentService
    {
        Task<APIResponse<UpdateParent>> AddParentAsync(Parents parent);
        async Task<APIResponse<UpdateParent>> UpdateParent(UpdateParent userDetail);
    }
}