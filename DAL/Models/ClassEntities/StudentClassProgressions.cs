using DAL.StudentInformation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ClassEntities
{
    public class StudentClassProgressions : CommonEntity
    {
        [Key]
        public Guid StudentClassProgressionId { get; set; }
        public Guid? SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public virtual SessionClass Class { get; set; } 
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual StudentContact Student { get; set; }
    }
}
