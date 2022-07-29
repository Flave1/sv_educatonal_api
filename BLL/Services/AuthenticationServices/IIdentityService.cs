using Contracts.Authentication;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IIdentityService
    {
        Task<APIResponse<LoginSuccessResponse>>  LoginAsync(LoginCommand user);
    }
}
