using DAL.SubjectModels;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Notes
{
    public class ShareNotes
    {
        public string ClassNoteId { get; set; }
        public Guid TeacherId { get; set; }
    }
    public class ClassNotes
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string SubjectId { get; set; }
        public bool ShouldSendForApproval { get; set; }
        public List<string> ClassId { get; set; }
    }
    public class UpdateClassNote
    { 
        public Guid ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string SubjectId { get; set; }
        public List<string> ClassId { get; set; }
    }
    public class GetClassNotes
    {
        public string ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string Author { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusName { get; set; }
        public string Subject { get; set; }
        public string Classes { get; set; }

        public GetClassNotes(ClassNote db)
        {
            ClassNoteId = db.ClassNoteId.ToString();
            NoteTitle = db.NoteTitle;
            NoteContent = db.NoteContent;
            Author = db.Author;
            ApprovalStatus = db.AprrovalStatus;
            if(ApprovalStatus == 1)
            {
                ApprovalStatusName = "Approved";
            }else if(ApprovalStatus == 2)
            {
                ApprovalStatusName = "Pending";
            }else if(ApprovalStatus == 3)
            {
                ApprovalStatusName = "In Progress";
            }
            else
            {
                ApprovalStatusName = "Not Approved";
            } 
            Subject = db.Subject.SubjectId.ToString();
            Classes = string.Join(',', db.Classes);

        }
    }
    public class ApproveClassNotes
    {
        public Guid ClassNoteId { get; set; }
        public bool ShouldApprove { get; set; }

    }
}
