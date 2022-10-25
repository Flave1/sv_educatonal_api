using DAL.TeachersInfor;
using Microsoft.AspNetCore.Identity;
using SMP.Contracts.Parents;
using System;

namespace DAL.Authentication
{
    public class AppUser : IdentityUser
    {
        public int UserType { get; set; } 
        public bool Active { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; } 
        public string DOB { get; set; }
        public string Photo { get; set; }
        /// <summary>
        //GENERAL
        /// </summary>
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public Teacher Teacher { get; set; }
        public Parents Parents { get; set; }
    }
}
