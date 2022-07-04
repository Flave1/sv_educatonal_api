using SMP.DAL.Models.GradeEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ClassEntities
{
    public class ClassLookup : CommonEntity
    { 
        [Key]
        public Guid ClassLookupId { get; set; }
        public string Name { get; set; }
        public Guid GradeGroupId { get; set; }
        [ForeignKey("GradeGroupId")]
        public GradeGroup GradeLevel { get; set; }
        public bool IsActive { get; set; }
    }
}
