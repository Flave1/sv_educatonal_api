using DAL;
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
    public class TeacherClassNoteComment: CommonEntity
    {
        [Key]
        public Guid TeacherClassNoteCommentId { get; set; }
        public string Comment { get; set; }
        public bool IsParent { get; set; }
        public Guid ClassNoteId { get; set; }
        [ForeignKey("ClassNoteId")]
        public ClassNote ClassNote { get; set; }
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public Guid? RepliedToId { get; set; }
        [ForeignKey("RepliedToId")]
        public TeacherClassNoteComment RepliedTo { get; set; }
        public virtual ICollection<TeacherClassNoteComment> Replies { get; set; }

    }
}
