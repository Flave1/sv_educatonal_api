using BLL.Constants;
using Contracts.Authentication;
using DAL.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IIdentityService
    {
        Task<APIResponse<LoginSuccessResponse>>  WebLoginAsync(LoginCommand user);
        Task<APIResponse<LoginSuccessResponse>> LoginAfterPasswordIsChangedAsync(AppUser userAccount, string schoolUrl, UserTypes uType);
        Task<APIResponse<LoginSuccessResponse>> MobileLoginAsync(LoginCommand loginRequest);
        Task<APIResponse<List<string>>> GetTeacherMobilePermissionsAsync(string userId);
        Task<APIResponse<CBTLoginDetails>> GetCBTTokenAsync();
    }
}
