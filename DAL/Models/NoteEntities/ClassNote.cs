using DAL;
using DAL.Authentication;
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

namespace SMP.DAL.Models.NoteEntities
{
    public class ClassNote : CommonEntity
    {
        [Key]
        public Guid ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public int AprrovalStatus { get; set; }
        public string Classes { get; set; }
        public string Author { get; set; }
        [ForeignKey("Author")]
        public AppUser AuthorDetail { get; set; }
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public virtual ICollection<TeacherClassNote> TeacherClassNotes { get; set; }
    }
}
