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
    public class ExamTimeTableDay: CommonEntity
    {
        [Key]
        public Guid ExamTimeTableDayId { get; set; }
        public string Day { get; set; }
        public Guid ExamTimeTableId { get; set; }
        [ForeignKey("ExamTimeTableId")]
        public ExamTimeTable ExamTimeTable { get; set; }
        public List<ExamTimeTableTimeActivity> Activities { get; set; }
    }
}
