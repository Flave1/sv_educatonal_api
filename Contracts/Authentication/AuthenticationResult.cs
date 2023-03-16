using DAL.Authentication;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;

namespace Contracts.Authentication
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string SchoolUrl { get; set; }
    }

    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SchoolUrl { get; set; }
    }

    public class UserDetail
    {
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string SchoolName { get; set; } = "";
        public string SchoolAbbreviation { get; set; } = "";
        public string SchoolLogo { get; set; } = "";
        public string Id { get; set; }
        public string UserAccountId { get; set; }
        public bool IsFirstTimeLogin { get; set; }
        public UserDetail() { }
        public UserDetail(SchoolSetting db, AppUser user, string FirstName, string LastName, Guid id)
        {
            Id = id.ToString();
            SchoolAbbreviation = db.SCHOOLSETTINGS_SchoolAbbreviation;
            SchoolLogo = db.SCHOOLSETTINGS_Photo;
            SchoolName = db.SCHOOLSETTINGS_SchoolName;
            UserName = FirstName + " " + LastName;
            UserAccountId = user.Id;
            IsFirstTimeLogin = !user.EmailConfirmed;
            if (user.UserType == -1)
                UserType = "Admin";
            if (user.UserType == 1)
                UserType = "Teacher";
            if (user.UserType == 0)
                UserType = "Student";
            if (user.UserType == 2)
                UserType = "Parent";
        }

    }
    public class PermissionsResponse
    {
        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class LoginSuccessResponse
    {
        public AuthenticationResult AuthResult { get; set; } = new AuthenticationResult();
        public UserDetail UserDetail { get; set; } = new UserDetail();
    }

    public class MobileLoginSuccessResponse
    {
        public AuthenticationResult AuthResult { get; set; } = new AuthenticationResult(); 
    }

    public class CBTLoginDetails
    {
        public AuthenticationResult AuthResult { get; set; }
        public UserDetails UserDetails { get; set; }
        public string ClientUrl { get; set; }
    }
    public class UserDetails
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
    }
    public class LoginCommandByHash
    {
        public string PasswordHash { get; set; }
    }
}

   
