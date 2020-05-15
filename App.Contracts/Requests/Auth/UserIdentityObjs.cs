using App.Contracts.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Contracts.Requests.Auth
{
    public class UserRegistrationReqObj
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class ChangePassword
    {
        [EmailAddress]
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserLoginReqObj
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserRefreshTokenReqObj
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }

    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }


    public class ConfirnmationRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class ConfirnmationResponse
    {
        public string Email { get; set; }
        public APIResponseStatus Status { get; set; }
    }

}
