using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTableDay : CommonEntity
    {
        [Key]
        public Guid ClassTimeTableDayId { get; set; }
        public string Day { get; set; }
        public Guid ClassTimeTableId { get; set; }
        [ForeignKey("ClassTimeTableId")]
        public ClassTimeTable TimeTable { get; set; }
    }
}
