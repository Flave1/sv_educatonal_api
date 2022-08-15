using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Attendance
{
    public class ClassActivity
    {
        public Guid ClassActivityId { get; set; }
        public string Activity { get; set; }
        public Guid ActivityDayId { get; set; }
        [ForeignKey("ActivityDayId")]
        public ActivityDay ActivityDay { get; set; }
        public Guid ActivityPeriodId { get; set; }
        [ForeignKey("ActivityPeriodId")]
        public ActivityPeriod ActivityPeriod { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact StudentContact { get; set; }
    }
}
