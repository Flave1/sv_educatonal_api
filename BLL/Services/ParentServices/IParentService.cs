using BLL;
using BLL.Filter;
using BLL.Wrappers;
using DAL.StudentInformation;
using SMP.Contracts.Parents;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SMP.BLL.Services.ParentServices
{
    public interface IParentService
    {
        Task<APIResponse<UpdateParent>> AddParentAsync(Parents parent);
        Task<APIResponse<UpdateParent>> UpdateParent(UpdateParent ParentDetail);
        Task<APIResponse<PagedResponse<List<Parents>>>> GetAllParentsAsync(PaginationFilter filter);
        Task<APIResponse<List<StudentDTO>>> GetAllStudentsByParentId(Parents parent);
    }
}