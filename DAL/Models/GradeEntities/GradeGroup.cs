using DAL;
using DAL.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.GradeEntities
{
    public class GradeGroup : CommonEntity
    {
        [Key]
        public Guid GradeGroupId { get; set; }
        public string GradeGroupName { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<ClassGrade> ClassGrades { get; set; }
    }

}
