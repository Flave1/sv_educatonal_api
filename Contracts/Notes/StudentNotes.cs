using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;

namespace SMP.Contracts.Notes
{
    public class StudentNotes
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public bool SubmitForReview { get; set; }
        public string SessionClassId { get; set; }
        public string SubjectId { get; set; }
        public string TeacherId { get; set; }
    }
    public class ApproveStudentNotes
    {
        public string StudentNoteId { get; set; }
        public bool ShouldApprove { get; set; }
    }
    public class UpdateStudentNote
    {
        public string SessionClassId { get; set; }
        public string StudentNoteId { get; set; }
        public string SubjectId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string TeacherId { get; set; }
    }
    public class ShareStudentNotes
    {
        public string StudentNoteId { get; set; }
        public List<Guid> StudentId { get; set; }
    }



    public class GetStudentNotes
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public bool ShouldSendForApproval { get; set; }
        public string SubjectId { get; set; }
        public Guid TeacherId { get; set; }
        public string StudentName { get; set; }
        public Guid SessionClassId { get; set; }
        public string SubjectName { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusName { get; set; }

        public GetStudentNotes(StudentNote db)
        {
            NoteTitle = db.NoteTitle;
            NoteContent = db.NoteContent;
            SubjectId = db.SubjectId.ToString();
            TeacherId = db.TeacherId;
            StudentName = db.Student.User.FirstName + " " + db.Student.User.LastName;
            SessionClassId = db.SessionClassId;
            SubjectName = db.Subject.Name.ToString();
            ApprovalStatus = db.AprrovalStatus;

            if (ApprovalStatus == 1)
                ApprovalStatusName = "Approved";
            else if (ApprovalStatus == 2)
                ApprovalStatusName = "Pending";
            else if (ApprovalStatus == 3)
                ApprovalStatusName = "In Progress";
            else
                ApprovalStatusName = "Not Approved";
        }
    }
}
