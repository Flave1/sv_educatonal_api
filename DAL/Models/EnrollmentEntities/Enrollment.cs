using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.EnrollmentEntities
{
    public class Enrollment: CommonEntity
    {
        [Key]
        public Guid EnrollmentId { get; set; }
        public int Status { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact Student { get; set; } 
    }
}
