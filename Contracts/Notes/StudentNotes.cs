using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.Contracts.Notes
{
    public class StudentNotes
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public bool SubmitForReview { get; set; }
        public string SubjectId { get; set; }
        public string TeacherId { get; set; }
    }
    public class ReviewStudentNoteRequest
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
        public bool SubmitForReview { get; set; }
    }

    public class AddCommentToStudentNote
    {
        public string StudentNoteId { get; set; }
        public string Comment { get; set; }
    }

    public class SendStudentNote
    {
        public string StudentNoteId { get; set; }
    }



    public class StudentNoteComments
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public Guid StudentNoteId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? RepliedToId { get; set; }
        public List<StudentNoteComments> RepliedComments { get; set; }
        public StudentNoteComments() { }
        public StudentNoteComments(StudentNoteComment db)
        {
            CommentId = db.StudentNoteCommentId;
            Comment = db.Comment;
            IsParent = db.IsParent;
            StudentNoteId = db.StudentNoteId;
            RepliedToId = db.RepliedToId;
            StudentId = db.StudentNote.StudentContactId;
            Name = db.User?.FirstName + "  " + db.User?.LastName;
            if (db.Replies is not null && db.Replies.Any())
            {
                RepliedComments = db.Replies.Select(x => new StudentNoteComments(x)).ToList();
            }
        }
    }


    public class ReplyCommentToStudentNote
    {
        public string CommentId { get; set; }
        public string Comment { get; set; }
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
        public Guid StudentNoteId { get; set; }
        public string DateSubmitted { get; set; }

        public GetStudentNotes(StudentNote db)
        {
            StudentNoteId = db.StudentNoteId;
            NoteTitle = db.NoteTitle;
            NoteContent = db.NoteContent;
            SubjectId = db.SubjectId.ToString();
            TeacherId = db.TeacherId;
            StudentName = db.Student.User.FirstName + " " + db.Student.User.LastName;
            SessionClassId = db.SessionClassId;
            SubjectName = db.Subject.Name.ToString();
            ApprovalStatus = db.AprrovalStatus;
            DateSubmitted = db.CreatedOn.ToString("dd/MM/yyy hh:mm");
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
