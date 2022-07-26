using System;
using System.Collections.Generic;

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

    public class SchoolPropertiesResponse
    {
        public string SchoolName { get; set; } = "";
        public string SchoolAbbreviation { get; set; } = "";
        public string SchoolLogo { get; set; } = "";

    }
    public class PermissionsResponse
    {
        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class LoginSuccessResponse
    {
        public AuthenticationResult AuthResult { get; set; } = new AuthenticationResult();
        public PermissionsResponse Permissions { get; set; } = new PermissionsResponse();
        public SchoolPropertiesResponse SchoolProperties { get; set; } = new SchoolPropertiesResponse();
    }
}

   
