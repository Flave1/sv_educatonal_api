using Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IRolesService
    {
        Task CreateRoleAsync(string roleName);
        Task<List<ApplicationRoles>> GetAllRolesAsync();
        Task<ApplicationRoles> GetSingleRoleAsync(string roleId);
        Task UpdateRoleAsync(string roleName, string roleId);
        Task DeleteRoleAsync(string roleId); 
    }
}
