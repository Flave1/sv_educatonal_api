using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.GradeEntities
{
    public class Grade: CommonEntity
    {
        [Key]
        public Guid GradeId { get; set; }
        public string GradeName { get; set; }
        public string Remark { get; set; }
        public Guid GradeGroupId { get; set; }
        [ForeignKey("GradeGroupId")]
        public GradeGroup GradeGroup { get; set; }
        public int UpperLimit { get; set; }
        public int LowerLimit { get; set; }
    }
}
