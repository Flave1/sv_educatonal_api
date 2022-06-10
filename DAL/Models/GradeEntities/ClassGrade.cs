using DAL;
using DAL.ClassEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.GradeEntities
{
    public class ClassGrade : CommonEntity
    {
        public Guid ClassGradeId { get; set; }
        public Guid GradeGroupId { get; set; }
        [ForeignKey("GradeGroupId")]
        public GradeGroup GradeGroup { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
    }
}
