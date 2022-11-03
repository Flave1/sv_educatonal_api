using BLL;
using BLL.Filter;
using BLL.Wrappers;
using SMP.Contracts.ParentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public interface IParentService
    {
        Task<APIResponse<PagedResponse<List<MyWards>>>> GetMyWardsAsync(PaginationFilter filter);
        Task<Guid> SaveParentDetail(string email, string name, string relationship, string number, Guid id);
    }
}