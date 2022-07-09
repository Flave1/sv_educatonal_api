using Contracts.Authentication;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginAsync(LoginCommand user);
    }
}
