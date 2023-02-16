using DAL.TeachersInfor;
using Microsoft.AspNetCore.Identity;
using System;

namespace DAL.Authentication
{
    public class AppUser : IdentityUser
    {
        public int UserType { get; set; } 
        public bool Active { get; set; } 
        
        /// <summary>
        //GENERAL
        /// </summary>
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public Teacher Teacher { get; set; }
        //public string ClientId { get; set; }
    }
}
