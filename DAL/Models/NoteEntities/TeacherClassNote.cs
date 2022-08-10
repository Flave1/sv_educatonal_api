using DAL;
using DAL.TeachersInfor;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMP.DAL.Models.NoteEntities
{
    public class TeacherClassNote : CommonEntity
    {
        [Key]
        public Guid TeacherClassNoteId { get; set; }
        public Guid ClassNoteId { get; set; }
        [ForeignKey("ClassNoteId")]
        public ClassNote ClassNote { get; set; }
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public string Classes { get; set; }
    }
}
