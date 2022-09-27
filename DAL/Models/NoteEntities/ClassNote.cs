using DAL;
using DAL.Authentication;
using DAL.SubjectModels;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.NoteEntities
{
    public class ClassNote : CommonEntity
    {
        [Key]
        public Guid ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public int AprrovalStatus { get; set; }
        public DateTime? DateSentForApproval { get; set; }
        public string Author { get; set; }
        [ForeignKey("Author")]
        public AppUser AuthorDetail { get; set; }
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public virtual ICollection<TeacherClassNote> TeacherClassNotes { get; set; }

        public Guid? SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }
    }
}
