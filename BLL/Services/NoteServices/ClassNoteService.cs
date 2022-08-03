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

        async Task<APIResponse<ShareNotes>> IClassNoteService.ShareClassNotesAsync(ShareNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<ShareNotes>(); 
            var sharedNoted = await context.ClassNote.FindAsync(Guid.Parse(request.ClassNoteId));
            if(sharedNoted is not null)
            {
                var newClassNote = new TeacherClassNote()
                {
                    ClassNoteId = sharedNoted.ClassNoteId,
                    TeacherId = Guid.Parse(string.Join(',',request.TeacherId))
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
            var note = await context.ClassNote.FindAsync(request.ClassNoteId);
            if (note != null)
            {
                note.AprrovalStatus = request.ShouldApprove ? (int)NoteApprovalStatus.Approved : (int)NoteApprovalStatus.NotApproved;
                await context.SaveChangesAsync();
            }
            else
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            res.IsSuccessful = true;
            if (note.AprrovalStatus == 1)
            { 
                res.Message.FriendlyMessage = "Approved Successfully";
            }
            else
            {
                res.Message.FriendlyMessage = "Not Approved Successful";
            }
            return res;
        }

        async Task<APIResponse<ClassNotes>> IClassNoteService.CreateClassNotesAsync(ClassNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<ClassNotes>();
            var subject = await context.Subject
                .Include(x => x.ScoreEntry)
                .ThenInclude(x => x.StudentContact)
                .ThenInclude(x => x.SessionClass)
                .FirstOrDefaultAsync(f=>f.SubjectId == Guid.Parse(request.SubjectId)); 
              
                var newClassNote = new ClassNote()
                {
                    NoteTitle = request.NoteTitle,
                    NoteContent = request.NoteContent,
                    AprrovalStatus = request.ShouldSendForApproval ? (int)NoteApprovalStatus.Pending : (int)NoteApprovalStatus.InProgress,
                    Author = userid,
                    SubjectId = subject.SubjectId,
                    Classes = string.Join(',' ,request.ClassId)
                };

                await context.ClassNote.AddAsync(newClassNote);
                await context.SaveChangesAsync();
             
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<bool>> IClassNoteService.DeleteClassNotesAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var note = await context.ClassNote.FindAsync(Guid.Parse(request.Item));
            if(note != null)
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

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetAllNotApprovedClassNotesAsync()
        {
            var res = new APIResponse<List<GetClassNotes>>();
            var noteList = await context.ClassNote.Include(x => x.Subject).Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.NotApproved).Select(x => new GetClassNotes(x)).ToListAsync();
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = noteList;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetClassNotesAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(userid))
            { 
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {

                    var noteList = await context.ClassNote.Include(x => x.Subject).Where(u => u.Deleted == false).Select(x => new GetClassNotes(x)).ToListAsync();
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.Result = noteList;
                }
                else
                {
                    var noteList = await context.ClassNote
                        .Include(x => x.Subject)
                        .ThenInclude(x => x.ScoreEntry)
                        .ThenInclude(x => x.StudentContact)
                        .ThenInclude(x => x.SessionClass)
                        .Where(u => u.Deleted == false && u.Subject.ScoreEntry.StudentContact.SessionClass.Teacher.TeacherId == Guid.Parse(teacherid))?.Select(x => new GetClassNotes(x)).ToListAsync();


                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.Result = noteList;
                }
            }
            else
            {

                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            return res;
        }

        async Task<APIResponse<UpdateClassNote>> IClassNoteService.UpdateClassNotesAsync(UpdateClassNote request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var res = new APIResponse<UpdateClassNote>();
            foreach (var classLookId in request.ClassId)
            {
                var classLookUp = await context.ClassLookUp.FirstOrDefaultAsync(x => x.ClassLookupId == Guid.Parse(classLookId));
                var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == request.ClassNoteId);
                if (note == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }
                note.ClassNoteId = request.ClassNoteId;
                note.NoteTitle = request.NoteTitle;
                note.NoteContent = request.NoteContent;
                note.Author = userid;
                note.AprrovalStatus = (int)NoteApprovalStatus.Pending;
                note.SubjectId =Guid.Parse(request.SubjectId);
                note.Classes = classLookUp.ClassLookupId.ToString();

                await context.SaveChangesAsync();

                res.Message.FriendlyMessage = Messages.Updated;
                res.IsSuccessful = true;
                res.Result = request;
            }
            return res;
             
        }
    }
}
