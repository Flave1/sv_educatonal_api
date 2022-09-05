using DAL;
using DAL.Authentication;
using DAL.ClassEntities;
using DAL.StudentInformation;
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
    public class StudentNoteComment: CommonEntity
    {
        [Key]
        public Guid StudentNoteCommentId { get; set; }
        public string Comment { get; set; }
        public bool IsParent { get; set; }
        public Guid StudentNoteId { get; set; }
        [ForeignKey("StudentNoteId")]
        public StudentNote StudentNote { get; set; }
        public Guid? RepliedToId { get; set; }
        [ForeignKey("RepliedToId")]
        public StudentNoteComment RepliedTo { get; set; }
        public virtual ICollection<StudentNoteComment> Replies { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

    }
}
