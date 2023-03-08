using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ClassEntities;
using SMP.DAL.Models.SessionEntities;

namespace SMP.DAL.Models.Timetable
{
    public class ExamTimeTable: CommonEntity
    {
        [Key]
        public Guid ExamTimeTableId { get; set; }
        public Guid ClassId { get; set; }
        [ForeignKey("ClassId")]
        public ClassLookup Class { get; set; }
        public virtual ICollection<ExamTimeTableDay> Days { get; set; }

        public virtual ICollection<ExamTimeTableTime> Times { get; set; }
    }
}
