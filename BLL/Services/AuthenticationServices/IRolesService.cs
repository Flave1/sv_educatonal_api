using Contracts.Authentication;
using Contracts.Common;
using DAL.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IRolesService
    {
        Task<APIResponse<UserRole>> CreateRoleAsync(CreateRoleActivity request);
        Task<APIResponse<List<ApplicationRoles>>> GetAllRolesAsync();
        Task<APIResponse<GetRoleActivities>> GetSingleRoleAsync(string roleId);
        Task<APIResponse<UserRole>> UpdateRoleAsync(UpdateRoleActivity request);
        Task<APIResponse<UserRole>> DeleteRoleAsync(MultipleDelete request);
        Task<APIResponse<List<GetActivities>>> GetAllActivitiesAsync();
        Task<APIResponse<List<GetActivityParent>>> GetActivityParentsAsync();
        Task<APIResponse<NotAddedUserRole>> GetUsersNotInRoleAsync(string roleId);
        Task<APIResponse<bool>> RemoveUserFromRoleAsync(RemoveUserFromRoleRequest request);
        Task<APIResponse<GetUsersInRole>> GetUsersInRoleAsync(GetUsersInRoleRequest request);
    }
}
