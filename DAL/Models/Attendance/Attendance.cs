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

namespace SMP.DAL.Models.Attendance
{
    public class Attendance: CommonEntity
    {
        [Key]
        public Guid ClassAttendanceId { get; set; }
        public Guid ClassRegisterId { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
    }
}
