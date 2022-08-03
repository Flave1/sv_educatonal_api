using DAL.Authentication;
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
        public List<string> Classes { get; set; }
    }
    public class GetClassNotes
    {
        public string ClassNoteId { get; set; }
        public string TeacherClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusName { get; set; }
        public string Subject { get; set; }
        public string SubjectName { get; set; }
        public List<string> Classes { get; set; }

        public GetClassNotes(TeacherClassNote db)
        {
            ClassNoteId = db.ClassNoteId.ToString();
            TeacherClassNoteId = db.TeacherClassNoteId.ToString();
            NoteTitle = db.ClassNote.NoteTitle;
            NoteContent = db.ClassNote.NoteContent;
            Author = db.ClassNote.Author.ToString();
            AuthorName = db.Teacher.User.FirstName + " " + db.Teacher.User.LastName;
            ApprovalStatus = db.ClassNote.AprrovalStatus;

            if(ApprovalStatus == 1)
                ApprovalStatusName = "Approved";
            else if(ApprovalStatus == 2)
                ApprovalStatusName = "Pending";
            else if(ApprovalStatus == 3)
                ApprovalStatusName = "In Progress";
            else
                ApprovalStatusName = "Not Approved";

            SubjectName = db.ClassNote.Subject.Name.ToString();
            Subject = db.ClassNote.SubjectId.ToString();
            Classes = db.ClassNote.Classes.Split().ToList();

        }
    }
    public class ApproveClassNotes
    {
        public string ClassNoteId { get; set; }
        public bool ShouldApprove { get; set; }

    }
}
