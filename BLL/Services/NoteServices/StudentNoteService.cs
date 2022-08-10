using BLL;
using BLL.Constants;
using Contracts.Authentication;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.Notes;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NoteServices
{
    public class StudentNoteService : IStudentNoteService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public StudentNoteService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<StudentNotes>> IStudentNoteService.CreateStudentNotesAsync(StudentNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value; 
            var res = new APIResponse<StudentNotes>();
            var sessionClass = await context.SessionClass
                .Include(u=>u.Students)
                .Include(u=>u.Teacher)
                .Include(u=>u.SessionClassSubjects)
                .FirstOrDefaultAsync(x => x.SessionClassId == request.SessionClassId);
            var subject = sessionClass.SessionClassSubjects.FirstOrDefault(x=>x.SessionClassId == request.SessionClassId);
            var newStudentNote = new StudentNote()
            {
                NoteTitle = request.NoteTitle,
                NoteContent = request.NoteContent,
                AprrovalStatus = request.ShouldSendForApproval ? (int)NoteApprovalStatus.InProgress : (int)NoteApprovalStatus.Saved,
                StudentContactId = sessionClass.Students.FirstOrDefault(x=>x.UserId == userid).StudentContactId,
                SubjectId =subject.SubjectId,
                TeacherId = sessionClass.Teacher.TeacherId,
                SessionClassId = request.SessionClassId
            };
             
            await context.StudentNote.AddAsync(newStudentNote);
            await context.SaveChangesAsync();
            
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<bool>> IStudentNoteService.DeleteStudentNotesAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var note = await context.StudentNote.FirstOrDefaultAsync(x=>x.StudentNoteId == Guid.Parse(request.Item));
            if (note != null)
            {
                note.Deleted = true;
                await context.SaveChangesAsync();
            }
            else
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            res.IsSuccessful = true;
            res.Result = true;
            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            return res;
        }
         

        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetStudentNotesByTeachersAsync(string subjectId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(teacherid))
            {
                if (!string.IsNullOrEmpty(subjectId))
                {
                    res.Result = await context.StudentNote
                    .Include(d => d.Teacher).ThenInclude(d => d.User) 
                         .Where(u => u.Deleted == false
                         && u.TeacherId == Guid.Parse(teacherid)
                         && u.SubjectId == Guid.Parse(subjectId))
                         .Select(x => new GetStudentNotes(x)).ToListAsync();
                }
                else
                {
                    res.Result = await context.StudentNote
                   .Include(d => d.Teacher).ThenInclude(d => d.User)
                        .Where(u => u.Deleted == false
                        && u.TeacherId == Guid.Parse(teacherid))
                        .Select(x => new GetStudentNotes(x)).ToListAsync();
                }
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetStudentNotesByAdminAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value; 
            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                var noteList = await context.StudentNote
                          .Include(e => e.Subject)
                          .Include(e => e.Student)
                          .ThenInclude(e => e.User)
                          .Include(e => e.SessionClass)
                         .Where(u => u.Deleted == false 
                         && u.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                         .Select(x => new GetStudentNotes(x)).ToListAsync();

                res.Result = noteList;
              
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetAllApprovalInProgressNoteAsync()
        {
            var res = new APIResponse<List<GetStudentNotes>>();

            var noteList = await context.StudentNote
                    .Include(e => e.Subject)
                    .Include(e => e.Student)
                    .ThenInclude(e => e.User)
                    .Include(e => e.SessionClass)
                    .Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                    .Select(x => new GetStudentNotes(x)).ToListAsync();

            res.Result = noteList;

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        async Task<APIResponse<bool>> IStudentNoteService.ApproveOrDisapproveStudentNotesAsync(ApproveStudentNotes request)
        { 
            var res = new APIResponse<bool>();
            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.Deleted == false && d.StudentNoteId == Guid.Parse(request.StudentNoteId));
            if (note != null)
            {
                note.AprrovalStatus = request.ShouldApprove ? (int)NoteApprovalStatus.Approved : (int)NoteApprovalStatus.NotApproved;
                await context.SaveChangesAsync();
            }
            else
            {
                res.Message.FriendlyMessage = "Student Note not found";
                return res;
            }

            res.IsSuccessful = true;
            if (note.AprrovalStatus == 1)
            {

                res.Result = true;
                res.Message.FriendlyMessage = "Approved Successfully";
            }
            else
            { 
                res.Result = true;
                res.Message.FriendlyMessage = "Not Approved Successful";
            }
            return res;
        }

        async Task<APIResponse<UpdateStudentNote>> IStudentNoteService.UpdateStudentNotesAsync(UpdateStudentNote request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<UpdateStudentNote>();

            var sessionClass = await context.SessionClass
                .Include(u => u.Students)
                .Include(u => u.Teacher)
                .Include(u => u.SessionClassSubjects)
                .FirstOrDefaultAsync(x => x.SessionClassId == Guid.Parse(request.SessionClassId));
            var subject = sessionClass.SessionClassSubjects.FirstOrDefault(x => x.SessionClassId == Guid.Parse(request.SessionClassId));
            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.StudentNoteId == request.StudentNoteId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            note.NoteTitle = request.NoteTitle;
            note.NoteContent = request.NoteContent;
            note.SubjectId = subject.SubjectId;
            note.StudentContactId = sessionClass.Students.FirstOrDefault(x=>x.UserId == userid).StudentContactId;
            note.SessionClassId = Guid.Parse(request.SessionClassId);
             

            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
             
        }

        async Task<APIResponse<GetStudentNotes>> IStudentNoteService.GetSingleStudentNotesAsync(SingleStudentNotes request)
        {
            var res = new APIResponse<GetStudentNotes>();
            res.Result = await context.StudentNote
                .Include(e => e.Subject)
                .Include(e => e.Student)
                .ThenInclude(e => e.User)
                .Include(e => e.SessionClass)
                .Where(u => u.Deleted == false 
                && u.StudentNoteId == Guid.Parse(request.StudentNoteId)) 
                .Select(x => new GetStudentNotes(x)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetStudentNotesByStudentAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                res.Result = await context.StudentNote
                .Include(e => e.Subject)
                .Include(e => e.Student).ThenInclude(e => e.User)
                .Include(e => e.SessionClass)
                .Where(u => u.Deleted == false
                && u.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                //&& u.Subject.ScoreEntry.StudentContact.User.Id == userid)
                .Select(x => new GetStudentNotes(x)).ToListAsync();

            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetSingleStudentNotesByStudentAsync(SingleStudentNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                res.Result = await context.StudentNote
                .Include(e => e.Subject) 
                .Include(e => e.Student).ThenInclude(e => e.User)
                .Include(e => e.SessionClass)
                //.Include(e => e.Teacher)
                .Where(u => u.Deleted == false
                && u.AprrovalStatus == (int)NoteApprovalStatus.Approved
                && u.StudentNoteId == Guid.Parse(request.StudentNoteId))
                .Select(x => new GetStudentNotes(x)).ToListAsync();

            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetSingleStudentNotesByAdminAsync(SingleStudentNotes request)
        {

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                res.Result = await context.StudentNote
                    .Include(e => e.Subject)
                    .Include(e => e.Student).ThenInclude(e => e.User)
                    .Include(e => e.SessionClass)
                    .Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.Approved && u.StudentNoteId == Guid.Parse(request.StudentNoteId))
                    .Select(x => new GetStudentNotes(x)).ToListAsync();
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<string>> IStudentNoteService.AddCommentToStudentNoteAsync(Guid studentNoteId, string comment)
        {
            var res = new APIResponse<string>();

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.StudentNoteId == studentNoteId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var commented = new StudentNoteComment
            {
                StudentNoteId = studentNoteId,
                Comment = comment,
                IsParent = true,
                StudentNote = note
            };

            context.StudentNoteComment.Add(commented);
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = "Comment sent succesfully";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<string>> IStudentNoteService.ReplyStudentNoteCommentAsync(string comment, Guid commentId)
        {
            var res = new APIResponse<string>();

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var note = await context.StudentNoteComment.FirstOrDefaultAsync(d => d.StudentNoteCommentId == commentId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var commented = new StudentNoteComment
            {
                StudentNoteId = note.StudentNoteId,
                Comment = comment,
                StudentNote = note.StudentNote,
                RepliedToId = commentId
            };

            context.StudentNoteComment.Add(commented);
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<List<StudentNoteComments>>> IStudentNoteService.GetStudentNoteCommentsAsync(string studentNoteId)
        {
            var res = new APIResponse<List<StudentNoteComments>>
            {
                Result = await context.StudentNoteComment
                .Include(d=>d.StudentNote)
                .Include(d => d.Replies).ThenInclude(d => d.RepliedTo)
                .Include(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(d => d.Replies)
                .Where(u => u.Deleted == false && u.StudentNoteId == Guid.Parse(studentNoteId) && u.IsParent == true)
                .Select(x => new StudentNoteComments(x)).ToListAsync(),

                IsSuccessful = true
            };
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    }
}
