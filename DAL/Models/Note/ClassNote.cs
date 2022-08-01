using DAL.ClassEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using DAL.TeachersInfor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Note
{
    public class ClassNote
    {
        [Key]
        public Guid ClassNoteId { get; set; }
        public string ClassNoteDetails { get; set; }
        public bool IsApproved { get; set; } = false;
        public string SeenByIds { get; set; }
        public bool IsShared { get; set; } = false;
        public bool IsSubmitted { get; set; } =false;   
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact Student { get; set; }
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher  Teacher { get; set; }
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public Guid ClassId { get; set; }
        public ICollection<ClassLookup> Class { get; set; }
    }
}
