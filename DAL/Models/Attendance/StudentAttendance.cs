using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Attendance
{
    public class StudentAttendance: CommonEntity
    {
        [Key]
        public Guid ClassAttendanceId { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
        public Guid ClassRegisterId { get; set; }
        [ForeignKey("ClassRegisterId")]
        public ClassRegister ClassRegister { get; set; }
    }
}
