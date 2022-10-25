using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SMP.Contracts.Parents
{
    public class Parents : AppUser
    {
        [Key]
        public string ParentId { get; set; }
        public int Status { get; set; }
        public string Relationship { get; set; }
        public ICollection<StudentContact> Students { get; set; }
        public IFormFile Photo { get; set; }
    }
    public class UpdateParent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public IFormFile Photo { get; set; }
        public string ImagePath { get; set }
        public string ParentId { get; set; }
        public string Email { get; set; }
    }
}