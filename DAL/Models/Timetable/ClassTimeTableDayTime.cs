using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTableDayTime: CommonEntity
    {
        [Key]
        public Guid ClassTimeTableDayTimeId { get; set; }
        public Guid ClassTimeTableDayId { get; set; }
        [ForeignKey("ClassTimeTableDayId")]
        public ClassTimeTableDay Day { get; set; }
        public virtual ICollection<ClassTimeTableDayActivity> Activities { get; set; }
    }
}
