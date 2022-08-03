using BLL;
using BLL.Constants;
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
    public class ClassNoteService : IClassNoteService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public ClassNoteService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<ClassNotes>> IClassNoteService.CreateClassNotesAsync(ClassNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<ClassNotes>();

            var newClassNote = new ClassNote()
            {
                NoteTitle = request.NoteTitle,
                NoteContent = request.NoteContent,
                AprrovalStatus = request.ShouldSendForApproval ? (int)NoteApprovalStatus.InProgress : (int)NoteApprovalStatus.Pending,
                Author = userid,
                SubjectId = Guid.Parse(request.SubjectId),
                Classes = string.Join(',', request.ClassId),
            };
            try
            {
                
                await context.ClassNote.AddAsync(newClassNote);
                await context.SaveChangesAsync();

                var teacherClassNote = new TeacherClassNote
                {
                    ClassNoteId = newClassNote.ClassNoteId,
                    TeacherId = Guid.Parse(teacherId)
                };

                await context.TeacherClassNote.AddAsync(teacherClassNote);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if(newClassNote is not null)
                {
                    context.ClassNote.Remove(newClassNote);
                    await context.SaveChangesAsync();
                }
                res.Message.FriendlyMessage = Messages.Created;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }


            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<bool>> IClassNoteService.DeleteClassNotesByAdminAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var note = await context.ClassNote.FindAsync(Guid.Parse(request.Item));
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

        async Task<APIResponse<bool>> IClassNoteService.DeleteTeacherClassNotesAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var note = await context.TeacherClassNote.FindAsync(Guid.Parse(request.Item));
            if (note != null)
            {
                context.TeacherClassNote.Remove(note);
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

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetClassNotesByTeachersAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                var noteList = await context.TeacherClassNote
                            .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                            .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                         .Where(u => u.Deleted == false && u.TeacherId == Guid.Parse(teacherid) 
                         && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                         .Select(x => new GetClassNotes(x)).ToListAsync();

                res.Result = noteList;
              
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetAllApprovalInProgressNoteAsync()
        {
            var res = new APIResponse<List<GetClassNotes>>();

            var noteList = await context.TeacherClassNote
                           .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                           .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                           .Include(x => x.Teacher).ThenInclude(x => x.User)
                        .Where(u => u.Deleted == false && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                        .Select(x => new GetClassNotes(x)).ToListAsync();

            res.Result = noteList;

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        async Task<APIResponse<ShareNotes>> IClassNoteService.ShareClassNotesAsync(ShareNotes request)
        {
            var res = new APIResponse<ShareNotes>(); 
            var noteToShare = await context.ClassNote.FindAsync(Guid.Parse(request.ClassNoteId));
            if(noteToShare is not null)
            {
                var newClassNote = new TeacherClassNote()
                {
                    ClassNoteId = noteToShare.ClassNoteId,
                    TeacherId = request.TeacherId
                };
                await context.TeacherClassNote.AddAsync(newClassNote);
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

        async Task<APIResponse<bool>> IClassNoteService.ApproveOrDisapproveClassNotesAsync(ApproveClassNotes request)
        { 
            var res = new APIResponse<bool>();
            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.Deleted == false && d.ClassNoteId == Guid.Parse(request.ClassNoteId));
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

        async Task<APIResponse<UpdateClassNote>> IClassNoteService.UpdateClassNotesAsync(UpdateClassNote request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<UpdateClassNote>();

            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == request.ClassNoteId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            note.NoteTitle = request.NoteTitle;
            note.NoteContent = request.NoteContent;
            note.SubjectId = Guid.Parse(request.SubjectId);
            note.Classes = string.Join(',', request.Classes);

            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;

            return res;
             
        }
    }
}
