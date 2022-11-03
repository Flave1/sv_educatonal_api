using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.Parents
{
    public class Parents
    {
        [Key]
        public Guid Parentid { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Relationship { get; set; }
        public string Email { get;set; }
        public string UserId { get; set; }

    }
}
