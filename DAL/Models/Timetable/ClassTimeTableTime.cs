using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTableTime: CommonEntity
    {
        [Key]
        public Guid ClassTimeTableTimeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public Guid ClassTimeTableId { get; set; }
        [ForeignKey("ClassTimeTableId")]
        public ClassTimeTable TimeTable { get; set; }

        public virtual ICollection<ClassTimeTableTimeActivity> Activities { get; set; }
    }
}
