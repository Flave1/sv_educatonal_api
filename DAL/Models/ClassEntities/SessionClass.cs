using DAL.SessionEntities;
using DAL.StudentInformation;
using DAL.TeachersInfor;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.NoteEntities;
using SMP.DAL.Models.Register;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ClassEntities
{
    public class SessionClass : CommonEntity
    {
        [Key]
        public Guid SessionClassId { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
        public Guid ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual ClassLookup Class { get; set; }
        public Guid? FormTeacherId { get; set; }
        [ForeignKey("FormTeacherId")]
        public virtual Teacher Teacher { get; set; }
        public Guid? ClassCaptainId { get; set; } 
        public bool InSession { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public int PassMark { get; set; }
        public ICollection<StudentContact> Students { get; set; }
        public ICollection<SessionClassSubject> SessionClassSubjects { get; set; }
        public ICollection<ClassRegister> ClassRegisters { get; set; }
        public ICollection<ClassScoreEntry> ClassScoreEntries { get; set; }
        public virtual ICollection<StudentNote> StudentNotes { get; set; }
        public virtual ICollection<SessionClassArchive> SessionClassArchive { get; set; }
    }
}
