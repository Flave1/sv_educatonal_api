using BLL.Filter;
using BLL.Wrappers;
using System.Linq;

namespace SMP.BLL.Services.FilterService
{
    public interface IPaginationService
    {
        PagedResponse<T> CreatePagedReponse<T>(T pagedData, PaginationFilter valFilter, int totalRecords);
        IQueryable<T> GetPagedResult<T>(IQueryable<T> query, PaginationFilter filter);
    }
}