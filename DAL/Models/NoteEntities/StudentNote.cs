using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using DAL.TeachersInfor;
using SMP.DAL.Models.SessionEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.NoteEntities
{
    public class StudentNote : CommonEntity
    {
        [Key]
        public Guid StudentNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public int AprrovalStatus { get; set; }
        public Guid StudentContactId { get; set; }
        [ForeignKey("StudentContactId")]
        public StudentContact Student { get; set; }  
        public Guid SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public Guid SessionClassId { get; set; }
        [ForeignKey("SessionClassId")]
        public SessionClass SessionClass { get; set; }
        public Guid? SessionTermId { get; set; }
        [ForeignKey("SessionTermId")]
        public SessionTerm SessionTerm { get; set; }

    }
}
