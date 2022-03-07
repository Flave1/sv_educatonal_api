using Contracts.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public interface IClassLookupService
    {
        Task CreateClassLookupAsync(string className);
        Task UpdateClassLookupAsync(string lookupName, string lookupId, bool isActive);
        Task<List<GetApplicationLookups>> GetAllClassLookupsAsync();
        Task DeleteClassLookupAsync(string lookupId);
    }
}
