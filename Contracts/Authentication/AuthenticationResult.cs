using System;

namespace Contracts.Authentication
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

   
