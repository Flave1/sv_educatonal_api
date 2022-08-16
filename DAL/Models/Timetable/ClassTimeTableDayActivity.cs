using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTableDayActivity
    {
        public Guid ClassTimeTableDayActivityId { get; set; }
        public string Activity { get; set; }
        public Guid ClassTimeTableDayTimeId { get; set; }
        [ForeignKey("ClassTimeTableDayTimeId")]
        public ClassTimeTableDayTime Time { get; set; }
    }
}
