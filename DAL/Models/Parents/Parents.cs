using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.Parents
{
    public class Parents : CommonEntity
    {
        [Key]
        public Guid Parentid { get; set; }
        public string Number { get; set; }
        public string Relationship { get; set; }
        public string Email { get;set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }

    }
}
