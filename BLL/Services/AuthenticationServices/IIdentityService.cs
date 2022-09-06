using Contracts.Authentication;
using DAL.Authentication;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IIdentityService
    {
        Task<APIResponse<LoginSuccessResponse>>  LoginAsync(LoginCommand user);
        Task<APIResponse<LoginSuccessResponse>> LoginAfterPasswordIsChangedAsync(AppUser userAccount);
    }
}
