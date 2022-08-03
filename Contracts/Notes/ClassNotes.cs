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
    public class GetTeacher
    {
        public Guid TeacherId { get; set; }
        public GetTeacher(TeacherClassNote db)
        {
            TeacherId = db.TeacherId;
        }
    }
    public class ShareNotes
    {
        public string ClassNoteId { get; set; }
        public List<Guid> TeacherId { get; set; }
    }
    public class SingleClassNotes
    {
        public string ClassNoteId { get; set; } 
    }
    public class SingleTeacherClassNotes
    {
        public string TeacherClassNoteId { get; set; } 
    }
    public class ClassNotes
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string SubjectId { get; set; }
        public bool ShouldSendForApproval { get; set; }
        public string ClassId { get; set; }
    }
    public class UpdateClassNote
    { 
        public Guid ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string SubjectId { get; set; }
        public string Classes { get; set; }
    }
    public class GetClassNotes
    {
        public string ClassNoteId { get; set; }
        public string TeacherClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public string NoteContent { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusName { get; set; }
        public string Subject { get; set; }
        public string SubjectName { get; set; }
        public string Classes { get; set; }

        public GetClassNotes(TeacherClassNote db)
        {
            ClassNoteId = db.ClassNoteId.ToString();
            TeacherClassNoteId = db.TeacherClassNoteId.ToString();
            NoteTitle = db.ClassNote.NoteTitle;
            DateCreated = db.CreatedOn;
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
            Classes = db.ClassNote.Classes;

        }
        public GetClassNotes(ClassNote db)
        {
            ClassNoteId = db.ClassNoteId.ToString(); 
            NoteTitle = db.NoteTitle;
            DateCreated = db.CreatedOn;
            NoteContent = db.NoteContent;
            Author = db.Author.ToString();
            AuthorName = db.AuthorDetail.FirstName + " " + db.AuthorDetail.LastName;
            ApprovalStatus = db.AprrovalStatus;

            if(ApprovalStatus == 1)
                ApprovalStatusName = "Approved";
            else if(ApprovalStatus == 2)
                ApprovalStatusName = "Pending";
            else if(ApprovalStatus == 3)
                ApprovalStatusName = "In Progress";
            else
                ApprovalStatusName = "Not Approved";

            SubjectName = db.Subject.Name.ToString();
            Subject = db.SubjectId.ToString();
            Classes = db.Classes;
        }
    }
    public class ApproveClassNotes
    {
        public string ClassNoteId { get; set; }
        public bool ShouldApprove { get; set; }

    }
}
