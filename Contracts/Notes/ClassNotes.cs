using DAL.ClassEntities;
using DAL.TeachersInfor;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class SendNote
    {
        public string TeacherClassNoteId { get; set; }
        public List<Guid> Classes { get; set; }
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

    public class AddCommentToClassNote
    {
        public string ClassNoteId { get; set; }
        public string Comment { get; set; }
    }

    public class ReplyCommentToClassNote
    {
        public string CommentId { get; set; }
        public string Comment { get; set; }
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
        public List<string> Classes { get; set; }
    }
    public class UpdateClassNote
    { 
        public Guid ClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
        public string SubjectId { get; set; }
    }
    public class GetClassNotes
    {
        public string ClassNoteId { get; set; }
        public string TeacherClassNoteId { get; set; }
        public string NoteTitle { get; set; }
        public string DateCreated { get; set; }
        public string NoteContent { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalStatusName { get; set; }
        public string Subject { get; set; }
        public string SubjectName { get; set; }
        public string[] Classes { get; set; }
        public string DateSentForApproval { get; set; }
        public NoteAuthordetail NoteAuthordetail { get; set; }

        public GetClassNotes(TeacherClassNote db, bool isSingle = false)
        {
            ClassNoteId = db.ClassNoteId.ToString();
            TeacherClassNoteId = db.TeacherClassNoteId.ToString();
            NoteTitle = db.ClassNote.NoteTitle;
            DateCreated = db.CreatedOn.ToString("dd-MM-yyy hh:mm");
            NoteContent = isSingle ? db.ClassNote.NoteContent : "" ;
            Author = db.ClassNote.Author.ToString();
            AuthorName = db.Teacher.User.FirstName + " " + db.Teacher.User.LastName;
            ApprovalStatus = db.ClassNote.AprrovalStatus;

            if(ApprovalStatus == 1)
                ApprovalStatusName = "Approved";
            else if(ApprovalStatus == 2)
                ApprovalStatusName = "Saved";
            else if(ApprovalStatus == 3)
                ApprovalStatusName = "In Progress";
            else
                ApprovalStatusName = "Not Approved";

            SubjectName = db.ClassNote.Subject.Name.ToString();
            Subject = db.ClassNote.SubjectId.ToString();
            Classes = !string.IsNullOrEmpty(db.Classes) ? db.Classes.Split(',').ToArray() : Array.Empty<string>();
            DateSentForApproval = db.ClassNote?.DateSentForApproval is not null ? db.ClassNote.DateSentForApproval.Value.ToString("dd-MM-yyy hh:mm") : "";
            NoteAuthordetail = isSingle ? new NoteAuthordetail
            {
                FullName = db.ClassNote.AuthorDetail.FirstName + " " + db.ClassNote.AuthorDetail.LastName,
                FirstName = db.ClassNote.AuthorDetail.FirstName,
                LastName = db.ClassNote.AuthorDetail.LastName,
                MiddleName = db.ClassNote.AuthorDetail.MiddleName,
                Photo = db.ClassNote.AuthorDetail.Photo,
                ShortBio = db.ClassNote.AuthorDetail.Teacher.ShortBiography
            } : null;
        }
        public GetClassNotes(ClassNote db)
        {
            ClassNoteId = db.ClassNoteId.ToString(); 
            NoteTitle = db.NoteTitle;
            DateCreated = db.CreatedOn.ToString("dd-MM-yyy hh:mm");
            NoteContent = db.NoteContent;
            Author = db.Author.ToString();
            AuthorName = db.AuthorDetail.FirstName + " " + db.AuthorDetail.LastName;

            ApprovalStatus = db.AprrovalStatus;

            if(ApprovalStatus == 1)
                ApprovalStatusName = "Approved";
            else if(ApprovalStatus == 2)
                ApprovalStatusName = "Saved";
            else if(ApprovalStatus == 3)
                ApprovalStatusName = "In Progress";
            else
                ApprovalStatusName = "Not Approved";

            SubjectName = db.Subject.Name.ToString();
            Subject = db.SubjectId.ToString();

            DateSentForApproval = db.DateSentForApproval.Value.ToString("dd-MM-yyy hh:mm");
            NoteAuthordetail = new NoteAuthordetail
            {
                FullName = db.AuthorDetail.FirstName + " " + db.AuthorDetail.LastName,
                FirstName = db.AuthorDetail.FirstName,
                LastName = db.AuthorDetail.LastName,
                MiddleName = db.AuthorDetail.MiddleName,
                Photo = db.AuthorDetail.Photo,
            };
        }
    }
    public class ApproveClassNotes
    {
        public string ClassNoteId { get; set; }
        public bool ShouldApprove { get; set; }

    }

    public class NoteAuthordetail
    {
        public string TeacherId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Photo { get; set; }
        public string ShortBio { get; set; }
    }

    public class ClassNoteTeachers
    {
        public string TeacherUserAccountId { get; set; }
        public string TeacherAccountId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool isShared { get; set; }
        public ClassNoteTeachers() { }
        public ClassNoteTeachers(Teacher db, bool isShare)
        {
            TeacherAccountId = db.TeacherId.ToString();
            TeacherUserAccountId = db.User.Id;
            FullName = db.User.FirstName + " " + db.User.LastName;
            FirstName = db.User.FirstName;
            LastName = db.User.LastName;
            MiddleName = db.User.MiddleName;
            isShared = isShare;
        }

        public ClassNoteTeachers(Teacher db)
        {
            TeacherAccountId = db.TeacherId.ToString();
            TeacherUserAccountId = db.User.Id;
            FullName = db.User.FirstName + " " + db.User.LastName;
            FirstName = db.User.FirstName;
            LastName = db.User.LastName;
            MiddleName = db.User.MiddleName;
            isShared = true;
        }
    }

    public class ClassNoteComment
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; }
        public bool IsParent { get; set; }
        public Guid ClassNoteId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid? RepliedToId { get; set; }
        public string Name { get; set; }
        public List<ClassNoteComment> RepliedComments { get; set; }
        public ClassNoteComment() { }
        public ClassNoteComment(TeacherClassNoteComment db)
        {
            CommentId = db.TeacherClassNoteCommentId;
            Comment = db.Comment;
            IsParent = db.IsParent;
            ClassNoteId = db.ClassNoteId;
            RepliedToId = db.RepliedToId;
            TeacherId = db.TeacherId;
            Name = db.Teacher.User.FirstName + " " + db.Teacher.User.LastName;
            if (db.Replies is not null && db.Replies.Any())
            {
                RepliedComments = db.Replies.Select(x => new ClassNoteComment(x)).ToList();
            }
        }
    }

    public class GetClasses2
    {
        public string SessionClass { get; set; }
        public string SessionClassId { get; set; }
        public bool IsSent { get; set; }
        public GetClasses2(SessionClass db, bool isSent)
        {
            SessionClass = db.Class.Name;
            SessionClassId = db.SessionClassId.ToString();
            IsSent = isSent;
        }
    }
}
