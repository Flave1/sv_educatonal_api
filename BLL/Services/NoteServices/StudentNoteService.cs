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

            


            var newStudentNote = new StudentNote()
            {
                NoteTitle = request.NoteTitle,
                NoteContent = request.NoteContent,
                AprrovalStatus = request.ShouldSendForApproval ? (int)NoteApprovalStatus.InProgress : (int)NoteApprovalStatus.Pending,
                StudentContactId = Guid.Parse(userid),
                SubjectId = Guid.Parse(request.SubjectId),
                TeacherId = request.TeacherId,
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
            var note = await context.StudentNote.FindAsync(Guid.Parse(request.Item));
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
                           .Include(x => x.Teacher).ThenInclude(x => x.User)
                        .Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                        .Select(x => new GetStudentNotes(x)).ToListAsync();

            res.Result = noteList;

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        async Task<APIResponse<ShareStudentNotes>> IStudentNoteService.ShareStudentNotesAsync(ShareStudentNotes request)
        {
                var res = new APIResponse<ShareStudentNotes>();
            try
            {
                var noteToShare = await context.StudentNote.FindAsync(Guid.Parse(request.StudentNoteId));
                if (noteToShare is not null)
                {
                    foreach (var student in request.StudentId)
                    {
                        var newStudentNote = new StudentNote()
                        {
                            StudentNoteId = noteToShare.StudentNoteId,
                            StudentContactId = student
                        };
                        await context.StudentNote.AddAsync(newStudentNote);
                    }
                    await context.SaveChangesAsync();
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Shared Successfully";
                res.Result = request;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
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
                res.Message.FriendlyMessage = "Note not found";
                return res;
            }

            res.IsSuccessful = true;
            if (note.AprrovalStatus == 1)
                res.Message.FriendlyMessage = "Approved Successfully";
            else
                res.Message.FriendlyMessage = "Not Approved Successful";
            return res;
        }

        async Task<APIResponse<UpdateStudentNote>> IStudentNoteService.UpdateStudentNotesAsync(UpdateStudentNote request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<UpdateStudentNote>();

            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.StudentNoteId == request.StudentNoteId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            note.NoteTitle = request.NoteTitle;
            note.NoteContent = request.NoteContent;
            note.SubjectId = Guid.Parse(request.SubjectId);
            note.StudentContactId = Guid.Parse(request.StudentContactId);

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
                .Include(d => d.Teacher).ThenInclude(d => d.User) 
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
                .ThenInclude(e => e.ScoreEntry)
                .ThenInclude(e => e.StudentContact)
                .ThenInclude(e => e.User)
                .Where(u => u.Deleted == false
                && u.AprrovalStatus == (int)NoteApprovalStatus.Approved
                && u.Subject.ScoreEntry.StudentContact.User.Id == userid)
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
                .ThenInclude(e => e.ScoreEntry)
                .ThenInclude(e => e.StudentContact)
                .ThenInclude(e => e.User)
                .Where(u => u.Deleted == false
                && u.AprrovalStatus == (int)NoteApprovalStatus.Approved
                && u.Subject.ScoreEntry.StudentContact.User.Id == userid && u.StudentNoteId == Guid.Parse(request.StudentNoteId))
                .Select(x => new GetStudentNotes(x)).ToListAsync();

            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetSingleStudentNotesByAdminAsync(SingleStudentNotes request)
        {
            var res = new APIResponse<List<GetStudentNotes>>();
            res.Result = await context.StudentNote
                .Include(d => d.Subject)         
                .Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.Approved && u.StudentNoteId == Guid.Parse(request.StudentNoteId))
                .Select(x => new GetStudentNotes(x)).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    }
}
