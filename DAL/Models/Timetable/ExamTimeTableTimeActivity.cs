using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Timetable
{
    public class ExamTimeTableTimeActivity: CommonEntity
    {
        [Key]
        public Guid ExamTimeTableTimeActivityId { get; set; }
        public string Activity { get; set; }
        public Guid ExamTimeTableTimeId { get; set; }
        [ForeignKey("ExamTimeTableTimeId")]
        public ExamTimeTableTime Time { get; set; }
        public Guid? ExamTimeTableDayId { get; set; }
        [ForeignKey("ExamTimeTableDayId")]
        public ExamTimeTableDay Day { get; set; }
    }
}
