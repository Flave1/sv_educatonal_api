using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Register
{
    public class Register: CommonEntity
    {
        [Key]  
        public Guid ClassRegisterId { get; set; }
        public Guid SessionClassId { get; set; }
        public Guid TakenBy { get; set; }   
        public string Label { get; set; }   
    }
}
