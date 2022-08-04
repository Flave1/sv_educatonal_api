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
    }

    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserDetail
    {
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string SchoolName { get; set; } = "";
        public string SchoolAbbreviation { get; set; } = "";
        public string SchoolLogo { get; set; } = "";
        public string Id { get; set; }
        public UserDetail() { }
        public UserDetail(SchoolSetting db, AppUser user, Guid id)
        {
            Id = id.ToString();
            SchoolAbbreviation = db.SchoolAbbreviation;
            SchoolLogo = db.Photo;
            SchoolName = db.SchoolName;
            UserName = user.FirstName + " " + user.LastName;
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
}

   
