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
    public class ActivityDay: CommonEntity
    {
        [Key]
        public Guid ActivityDayId { get; set; }
        public string Day { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
    }
}
