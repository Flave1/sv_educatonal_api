using Contracts.Class;
using Contracts.Common;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public interface IClassLookupService
    {
        Task<APIResponse<ClassLookup>> CreateClassLookupAsync(string className, Guid gradeLevelId);
        Task<APIResponse<ClassLookup>> UpdateClassLookupAsync(string lookupName, string lookupId, bool isActive, Guid gradeLevelId);
        Task<APIResponse<List<GetApplicationLookups>>> GetAllClassLookupsAsync();
        Task<APIResponse<ClassLookup>> DeleteClassLookupAsync(MultipleDelete lookupId);
        Task<APIResponse<List<GetApplicationLookups>>> GetAllActiveClassLookupsAsync();
    }
}
