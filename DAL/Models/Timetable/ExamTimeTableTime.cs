using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace SMP.DAL.Models.Timetable
{
    public class ExamTimeTableTime: CommonEntity
    {
        [Key]
        public Guid ExamTimeTableTimeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public Guid ExamTimeTableId { get; set; }
        [ForeignKey("ExamTimeTableId")]
        public ExamTimeTable ExamTimeTable { get; set; }

        public virtual ICollection<ExamTimeTableTimeActivity> Activities { get; set; }
    }
}
