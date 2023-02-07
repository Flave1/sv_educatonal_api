using DAL;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTableTimeActivity : CommonEntity
    {
        [Key]
        public Guid ClassTimeTableTimeActivityId { get; set; }
        public string Activity { get; set; }
        public Guid ClassTimeTableTimeId { get; set; }
        [ForeignKey("ClassTimeTableTimeId")]
        public ClassTimeTableTime Time { get; set; }
        public Guid? ClassTimeTableDayId { get; set; }
        [ForeignKey("ClassTimeTableDayId")]
        public ClassTimeTableDay Day { get; set; }
    }
}
