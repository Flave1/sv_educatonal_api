using App.Contracts.Requests.Auth;
using App.Contracts.Response;
using App.DomainObjects.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace App.AuthHandler.Interface
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(UserRegistrationReqObj userRegistration);
        Task<AuthenticationResult> ChangePasswsord(ChangePassword pass);
        Task<AuthenticationResult> LoginAsync(string userName, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string token);
        Task<bool> CheckUserAsync(string email);
        Task<UserDataResponseObj> FetchLoggedInUserDetailsAsync(string userId);
        Task<ConfirnmationResponse> ConfirmEmailAsync(ConfirnmationRequest request);
        Task<ConfirnmationResponse> VerifyAsync(string code);
    }
}
