using DAL;
using DAL.ClassEntities;
using SMP.DAL.Models.Attendance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Register
{
    public class ClassRegister: CommonEntity
    {
        [Key]  
        public Guid ClassRegisterId { get; set; }
        public string RegisterLabel { get; set; }  
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; } 
        public ICollection<StudentAttendance> StudentAttendances { get; set; }
    }
}
