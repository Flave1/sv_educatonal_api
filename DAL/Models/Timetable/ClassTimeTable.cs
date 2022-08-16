using DAL;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.Timetable
{
    public class ClassTimeTable : CommonEntity
    {
        [Key]
        public Guid ClassTimeTableId { get; set; }
        public Guid ClassId { get; set; }
        [ForeignKey("ClassId")]
        public ClassLookup Class { get; set; }
        public virtual ICollection<ClassTimeTableDay> Days { get; set; }
    }
}
