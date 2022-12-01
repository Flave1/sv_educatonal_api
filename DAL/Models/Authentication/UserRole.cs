using Microsoft.AspNetCore.Identity;
using System;

namespace DAL.Authentication
{
    public class UserRole : IdentityRole
    {
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
