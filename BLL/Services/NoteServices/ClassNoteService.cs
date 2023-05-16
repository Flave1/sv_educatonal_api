using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using DAL.StudentInformation;
using DAL.TeachersInfor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security.Provider;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using SMP.API.Hubs;
using SMP.BLL.Constants;
using SMP.BLL.Hubs;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.NotififcationServices;
using SMP.Contracts.Notes;
using SMP.Contracts.NotificationModels;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharpCore.Pdf.Advanced;
using System.IO;
using SMP.BLL.Services.SessionServices;

namespace SMP.BLL.Services.NoteServices
{
    public class ClassNoteService : IClassNoteService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly IHubContext<NotificationHub> hub;
        private readonly INotificationService notificationService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly ITermService termService;
        public ClassNoteService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService,
            IHubContext<NotificationHub> hub, INotificationService notificationService, ILoggerService loggerService, ITermService termService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.hub = hub;
            this.notificationService = notificationService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
        }

        async Task<APIResponse<ClassNotes>> IClassNoteService.CreateClassNotesAsync(ClassNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var termid = termService.GetCurrentTerm().SessionTermId;
            var res = new APIResponse<ClassNotes>();

            var newClassNote = new ClassNote()
            {
                NoteTitle = request.NoteTitle,
                NoteContent = request.NoteContent,
                AprrovalStatus = request.ShouldSendForApproval ? (int)NoteApprovalStatus.InProgress : (int)NoteApprovalStatus.Saved,
                Author = userid,
                SubjectId = Guid.Parse(request.SubjectId),
                SessionTermId = termid
            };
            try
            {
                
                await context.ClassNote.AddAsync(newClassNote);
                await context.SaveChangesAsync();

                var teacherClassNote = new TeacherClassNote
                {
                    ClassNoteId = newClassNote.ClassNoteId,
                    TeacherId = Guid.Parse(teacherId),
                    Classes = string.Join(',', request.Classes),
                    SessionTermId = termid
                };

                await context.TeacherClassNote.AddAsync(teacherClassNote);
                await context.SaveChangesAsync();

                if (request.ShouldSendForApproval)
                {
                    await SendNotificationToAdminAsync(userid, newClassNote.ClassNoteId, newClassNote.SubjectId);
                }
            }
            catch (Exception ex)
            {
                if(newClassNote is not null)
                {
                    context.ClassNote.Remove(newClassNote);
                    await context.SaveChangesAsync();
                }
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
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

        async Task<APIResponse<PagedResponse<List<GetClassNotes>>>> IClassNoteService.GetClassNotesByTeachersAsync(string classId, string subjectId, int status, string termId, PaginationFilter filter)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<PagedResponse<List<GetClassNotes>>>();

            IQueryable<TeacherClassNote> query = null;

            query = context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
           .Include(d => d.Teacher).ThenInclude(d => d.User)
                   .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                   .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                   .OrderByDescending(x => x.CreatedOn);

            if (status == -2)
            {
                query = query.Where(u => u.Deleted == false && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress);
            }
            else if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
            {
                query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
            }

            if (!string.IsNullOrEmpty(termId))
            {
                query = query.Where(u => Guid.Parse(termId) == u.ClassNote.SessionTermId);
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(u => Guid.Parse(subjectId) == u.ClassNote.SubjectId);
            }

            else if (status >= 0)
            {
                query = query.Where(u => status == u.ClassNote.AprrovalStatus);
            }
            if (!string.IsNullOrEmpty(classId))
            {
                var classes = query.Select(u => new { id = u.TeacherClassNoteId, cls = u.Classes }).AsEnumerable();
                var selectedClassNotes = classes.Where(x => !string.IsNullOrEmpty(x.cls) ? x.cls.Split(',').Any(c => c == classId) : false);
                query = query.Where(u => selectedClassNotes.Select(x => x.id).Contains(u.TeacherClassNoteId));
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetClassNotes(x, false)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<PagedResponse<List<GetClassNotes>>>> IClassNoteService.GetClassNotesByTeachersMobileAsync(string classId, string subjectId, int status, PaginationFilter filter)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<PagedResponse<List<GetClassNotes>>>();

            IQueryable<TeacherClassNote> query = null;
            var termId = termService.GetCurrentTerm().SessionTermId;

            query = context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
           .Include(d => d.Teacher).ThenInclude(d => d.User)
                   .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                   .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                   .OrderByDescending(x => x.CreatedOn);

            query = query.Where(u => termId == u.ClassNote.SessionTermId);
            if (status == -2)
            {
                query = query.Where(u => u.Deleted == false && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress);
            }
            else if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
            {
                query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
            }
            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(u => Guid.Parse(subjectId) == u.ClassNote.SubjectId);
            }

            else if (status >= 0)
            {
                query = query.Where(u => status == u.ClassNote.AprrovalStatus);
            }
            if (!string.IsNullOrEmpty(classId))
            {
                var classes = query.Select(u => new { id = u.TeacherClassNoteId, cls = u.Classes }).AsEnumerable();
                var selectedClassNotes = classes.Where(x => !string.IsNullOrEmpty(x.cls) ? x.cls.Split(',').Any(c => c == classId) : false);
                query = query.Where(u => selectedClassNotes.Select(x => x.id).Contains(u.TeacherClassNoteId));
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetClassNotes(x, true)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetClassNotesByStatusAsync(string subjectId, int status)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(teacherid))
            {
                if (!string.IsNullOrEmpty(subjectId))
                {
                    res.Result = await context.TeacherClassNote.Where(x => x.ClientId == smsClientId)
                    .Include(d => d.Teacher)
                            .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                            .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                            .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Saved)
                             .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                         .Where(u => u.Deleted == false
                         && u.ClassNote.AprrovalStatus == status
                         && u.TeacherId == Guid.Parse(teacherid)
                         && u.ClassNote.SubjectId == Guid.Parse(subjectId))
                         .Select(x => new GetClassNotes(x, false)).ToListAsync();
                }
                else
                {
                    res.Result = await context.TeacherClassNote.Where(x => x.ClientId == smsClientId)
                   .Include(d => d.Teacher).ThenInclude(d => d.User)
                           .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                           .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                            .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Saved)
                             .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                        .Where(u => u.Deleted == false
                           && u.ClassNote.AprrovalStatus == status
                        && u.TeacherId == Guid.Parse(teacherid))
                        .Select(x => new GetClassNotes(x, false)).ToListAsync();
                }
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetClassNotesByAdminAsync()
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value; 
            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(userid))
            {
                var noteList = await context.ClassNote
                         .Where(u => u.Deleted == false 
                         && u.AprrovalStatus == (int)NoteApprovalStatus.Approved && u.Author == userid && u.ClientId == smsClientId)
                         .Select(x => new GetClassNotes(x, context.Teacher.FirstOrDefault(c => c.UserId == x.Author))).ToListAsync();

                res.Result = noteList;
              
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetAllApprovalInProgressNoteAsync()
        {
            var res = new APIResponse<List<GetClassNotes>>();

            var noteList = await context.TeacherClassNote.Where(x => x.ClientId == smsClientId)
                           .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                           .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                           .Include(x => x.Teacher).ThenInclude(x => x.User)
                        .Where(u => u.Deleted == false && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                        .Select(x => new GetClassNotes(x, false)).ToListAsync();

            res.Result = noteList;

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        async Task<APIResponse<ShareNotes>> IClassNoteService.ShareClassNotesAsync(ShareNotes request)
        {
                var res = new APIResponse<ShareNotes>();
                var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var termid = termService.GetCurrentTerm().SessionTermId;

            try
            {
                var noteToShare = await context.ClassNote.FindAsync(Guid.Parse(request.ClassNoteId));
                if (noteToShare is not null)
                {
                    if(noteToShare.AprrovalStatus is not (int)NoteApprovalStatus.Approved){
                        res.Message.FriendlyMessage = "Unapproved Note cannot be shared";
                        return res;
                    }
                    //request.TeacherId.Add(Guid.Parse(teacherId));
                    var alreadySharedWith = context.TeacherClassNote.Where(x => x.ClassNoteId == Guid.Parse(request.ClassNoteId)).ToList();
                    if (alreadySharedWith.Any())
                    {
                        context.TeacherClassNote.RemoveRange(alreadySharedWith);
                        await context.SaveChangesAsync();
                    }

                    foreach (var teacher in request.TeacherId)
                    {
                        var newClassNote = new TeacherClassNote()
                        {
                            ClassNoteId = noteToShare.ClassNoteId,
                            TeacherId = teacher,
                            SessionTermId = termid
                        };
                        await context.TeacherClassNote.AddAsync(newClassNote);
                    }
                    await context.SaveChangesAsync();

                    await NotifyTeacherOnNoteShareAsync(noteToShare, request);
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
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        async Task<APIResponse<bool>> IClassNoteService.ApproveOrDisapproveClassNotesAsync(ApproveClassNotes request)
        { 
            var res = new APIResponse<bool>();
            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.Deleted == false && d.ClassNoteId == Guid.Parse(request.ClassNoteId) && d.ClientId == smsClientId);
            if (note != null)
            {
                note.AprrovalStatus = request.ShouldApprove ? (int)NoteApprovalStatus.Approved : (int)NoteApprovalStatus.NotApproved;
                await context.SaveChangesAsync();

                if(request.ShouldApprove)
                {
                    await NotifyTeacherOnNoteApprovalAsync(note);
                }
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

            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == request.ClassNoteId && d.ClientId == smsClientId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            note.NoteTitle = request.NoteTitle;
            note.NoteContent = request.NoteContent;
            note.SubjectId = Guid.Parse(request.SubjectId);

            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<GetClassNotes>> IClassNoteService.GetSingleTeacherClassNotesAsync(string TeacherClassNoteId)
        {
            var res = new APIResponse<GetClassNotes>();
            res.Result = await context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false
                && x.TeacherClassNoteId == Guid.Parse(TeacherClassNoteId))
                .Include(d => d.Teacher).ThenInclude(d => d.User)
                .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail).ThenInclude(x => x.Teacher)
                .Select(x => new GetClassNotes(x, true)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetSingleClassNotesByAdminAsync(SingleClassNotes request)
        {
            var res = new APIResponse<List<GetClassNotes>>();
            res.Result = await context.ClassNote.Where(u => u.ClientId == smsClientId && u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                .Include(d => d.Subject)
                .Select(x => new GetClassNotes(x, context.Teacher.FirstOrDefault(d => d.UserId == x.Author))).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<ClassNoteTeachers>>> IClassNoteService.GetTeachersNoteSharedToAsync(Guid classNoteId)
        {
            var res = new APIResponse<List<ClassNoteTeachers>>();

            var alreadyShared = context.ClassNote.Where(d => d.ClassNoteId == classNoteId && d.ClientId == smsClientId).Include(x => x.TeacherClassNotes).SelectMany(s => s.TeacherClassNotes).Select(x => x.TeacherId).ToList();
            res.Result = await context.Teacher.Where(d => d.Deleted == false && d.Status == (int)TeacherStatus.Active && d.ClientId == smsClientId).OrderByDescending(d => d.CreatedOn)
                 .Select(a => new ClassNoteTeachers(a, alreadyShared.Contains(a.TeacherId))).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<ClassNoteTeachers>>> IClassNoteService.GetOtherTeachersAsync(Guid classNoteId)
        {
            var res = new APIResponse<List<ClassNoteTeachers>>();

            var alreadyShared = context.ClassNote.Include(x => x.TeacherClassNotes).Where(d => d.ClassNoteId == classNoteId).SelectMany(s => s.TeacherClassNotes).Select(x => x.TeacherId).ToList();
            res.Result = await context.Teacher.Where(d => d.Deleted == false && d.Status == (int)TeacherStatus.Active && alreadyShared.Contains(d.TeacherId) && d.ClientId == smsClientId).OrderByDescending(d => d.CreatedOn)
                 .Select(a => new ClassNoteTeachers(a)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.SendClassNoteForAprovalAsync(Guid classNoteId)
        {
            var res = new APIResponse<string>();

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == classNoteId && d.ClientId == smsClientId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            if(note.AprrovalStatus != (int)NoteApprovalStatus.Saved && note.AprrovalStatus != (int)NoteApprovalStatus.NotApproved)
            {
                res.Message.FriendlyMessage = "Note Can Not Be Approved";
                return res;
            }

            note.AprrovalStatus = (int)NoteApprovalStatus.InProgress;
            note.DateSentForApproval = DateTime.UtcNow;
            await context.SaveChangesAsync();

            await SendNotificationToAdminAsync(userid, note.ClassNoteId, note.SubjectId);

            res.Message.FriendlyMessage = "Successfully sent for approval";
            res.IsSuccessful = true;
            res.Result = classNoteId.ToString();
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.AddCommentToClassNoteAsync(Guid classNoteId, string comment)
        {
            var res = new APIResponse<string>();

            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == classNoteId && d.ClientId == smsClientId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var commented = new TeacherClassNoteComment
            {
                ClassNoteId = classNoteId,
                Comment = comment,
                IsParent = true,
                UserId = userId,
            };

            context.TeacherClassNoteComment.Add(commented);
            await context.SaveChangesAsync();

            #region notification
            //var appActivityId = "13f6763e-e5ab-45a0-8da1-244dbe63d298"; /// >>> Pls do not change Id
            //var roleId = context.RoleActivity.FirstOrDefault(r => r.ActivityId == Guid.Parse(appActivityId) && r.Deleted == false).RoleId;
            //var userIds = context.UserRoles.Where(r => r.RoleId == roleId).Select(x => x.UserId).ToList();

            //string receiverEmail = "";
            //var subject = context.Subject.FirstOrDefault(m => m.SubjectId == note.SubjectId).Name;
            //var commenter = context.Users.FirstOrDefault(m => m.Id == userId).FirstName;

            //string toGroup = "";
            //if (userIds.Contains(userId))
            //{
            //    var teacherId = note.Author;
            //    receiverEmail = context.Users.FirstOrDefault(x => x.Id == teacherId).Email;
            //    toGroup = "Teachers";
            //}
            //else
            //{
            //    receiverEmail = string.Join(",", context.Users.Where(a => a.Deleted == false && userIds.Contains(a.Id)).Select(x => x.Email).ToList());
            //    toGroup = "Admin";
            //}


            //await notificationService.CreateNotitficationAsync(new NotificationDTO
            //{
            //    Content = $"{commenter} commented on {subject} note",
            //    NotificationPageLink = $"smp-notification/lesson-note-details?ClassNoteId={note.ClassNoteId}",
            //    NotificationSourceId = note.ClassNoteId.ToString(),
            //    Subject = "Class Note",
            //    ReceiversEmail = receiverEmail,
            //    Type = "class-note",
            //    ToGroup = toGroup
            //});
            //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());

            #endregion
            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.ReplyClassNoteCommentAsync(string comment, Guid commentId)
        {
            var res = new APIResponse<string>();

            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = await context.TeacherClassNoteComment.FirstOrDefaultAsync(d => d.TeacherClassNoteCommentId == commentId && d.ClientId == smsClientId);
            if (note == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var commented = new TeacherClassNoteComment
            {
                ClassNoteId = note.ClassNoteId,
                Comment = comment,
                UserId = userId,
                RepliedToId = commentId
            };

            context.TeacherClassNoteComment.Add(commented);
            await context.SaveChangesAsync();

            #region notification
            //var commenter = context.Users.FirstOrDefault(m => m.Id == userId).FirstName;
            //var subjectId = context.ClassNote.FirstOrDefault(m => m.ClassNoteId == note.ClassNoteId).SubjectId;
            //var subject = context.Subject.FirstOrDefault(m => m.SubjectId == subjectId).Name;
            //string receiverEmail = context.Users.FirstOrDefault(x => x.Id == note.UserId).Email;

            //var appActivityId = "13f6763e-e5ab-45a0-8da1-244dbe63d298"; /// >>> Pls do not change Id
            //var roleId = context.RoleActivity.FirstOrDefault(r => r.ActivityId == Guid.Parse(appActivityId) && r.Deleted == false).RoleId;
            //var userIds = context.UserRoles.Where(r => r.RoleId == roleId).Select(x => x.UserId).ToList();

            //string toGroup = "";
            //if (userIds.Contains(userId))
            //{
            //    toGroup = "Teachers";
            //}
            //else
            //{
            //    toGroup = "Admin";
            //}

            //await notificationService.CreateNotitficationAsync(new NotificationDTO
            //{
            //    Content = $"{commenter} commented on {subject} note",
            //    NotificationPageLink = $"smp-class/lesson-note-details?ClassNoteId={note.ClassNoteId}",
            //    NotificationSourceId = note.ClassNoteId.ToString(),
            //    Subject = "Student Note",
            //    Receivers = receiverEmail,
            //    Type = "class-note",
            //    ToGroup = toGroup
            //});
            //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());

            #endregion

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<List<ClassNoteComment>>> IClassNoteService.GetClassNoteCommentsAsync(string classNoteId)
        {
            var res = new APIResponse<List<ClassNoteComment>>();
            var parentComments = await context.TeacherClassNoteComment
                .Where(u => u.ClientId == smsClientId 
                && u.Deleted == false
                && u.ClassNoteId == Guid.Parse(classNoteId) 
                && u.IsParent == true).ToListAsync();


            var response = new List<ClassNoteComment>();
            for(var i = 0; i < parentComments.Count; i++)
            {
                var comment = parentComments[i];
                var teacher = context.Teacher.FirstOrDefault(x => x.UserId == comment.UserId && x.ClientId == smsClientId);
                var student = context.StudentContact.FirstOrDefault(x => x.UserId == comment.UserId && x.ClientId == smsClientId);

                var result = new ClassNoteComment(comment, teacher, student);
                var commentIsAparentComment = context.TeacherClassNoteComment.Any(x => x.RepliedToId ==  comment.TeacherClassNoteCommentId);
                if (commentIsAparentComment)
                {
                    result.RepliedComments = await GetChildCommentAsync(comment.TeacherClassNoteCommentId);
                }
                response.Add(result);
            }

            res.Result = response;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        private async Task<List<ClassNoteComment>> GetChildCommentAsync(Guid? commentId)
        {
            var query = await context.TeacherClassNoteComment.Where(x => x.TeacherClassNoteCommentId == commentId).ToListAsync();
            var response = new List<ClassNoteComment>();
            foreach (var item in query)
            {
                var teacher = await context.Teacher.FirstOrDefaultAsync(x => x.UserId == item.UserId && x.ClientId == smsClientId);
                var student = await context.StudentContact.FirstOrDefaultAsync(x => x.UserId == item.UserId && x.ClientId == smsClientId);
                var result = new ClassNoteComment(item, teacher, student);
                if (item.RepliedToId is not null)
                {
                    result.RepliedComments = await GetGrandChildCommentsAsync(item.RepliedToId);
                }
                response.Add(result);
            }
            return response;
        }
        private async Task<List<ClassNoteComment>> GetGrandChildCommentsAsync(Guid? commentId)
        {
            var query = await context.TeacherClassNoteComment.Where(x => x.TeacherClassNoteCommentId == commentId).ToListAsync();
            var response = new List<ClassNoteComment>();
            foreach (var item in query)
            {
                var teacher = await context.Teacher.FirstOrDefaultAsync(x => x.UserId == item.UserId && x.ClientId == smsClientId);
                var student = await context.StudentContact.FirstOrDefaultAsync(x => x.UserId == item.UserId && x.ClientId == smsClientId);
                var result = new ClassNoteComment(item, teacher, student);
                if (item.RepliedToId is not null)
                {
                    result.RepliedComments = await GetChildCommentAsync(item.RepliedToId);
                }
                response.Add(result);
            }
            return response;
        }
        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetRelatedClassNoteAsync(Guid classNoteId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = context.ClassNote.FirstOrDefault(d => d.ClassNoteId == classNoteId && d.ClientId == smsClientId);

            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(teacherid))
            {
                res.Result = await context.TeacherClassNote.Where(u => u.ClientId == smsClientId && u.Deleted == false && u.ClassNote.SubjectId == note.SubjectId
                         && u.ClassNote.AprrovalStatus != (int)NoteApprovalStatus.Saved
                         && u.ClassNote.AprrovalStatus != (int)NoteApprovalStatus.NotApproved
                         && u.TeacherId == Guid.Parse(teacherid))
                        .Include(d => d.Teacher).ThenInclude(d => d.User)
                         .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                         .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                          .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Saved)
                           .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                      .Select(x => new GetClassNotes(x, true)).ToListAsync();
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<SendNote>> IClassNoteService.SendClassNoteToClassesAsync(SendNote request)
        {
            var res = new APIResponse<SendNote>();
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            try
            {
                var noteToSend = context.TeacherClassNote.Where(x=>x.ClientId == smsClientId).Include(d => d.ClassNote).FirstOrDefault(x => x.TeacherClassNoteId == Guid.Parse(request.TeacherClassNoteId));
                if (noteToSend is not null)
                {
                    if(noteToSend.ClassNote.AprrovalStatus is not (int)NoteApprovalStatus.Approved)
                    {
                        res.Message.FriendlyMessage = "Unapproved note cannot be sent to students";
                        return res;
                    }
                    noteToSend.Classes = string.Join(',', request.Classes);

                    await context.SaveChangesAsync();

                    #region notification
                    //var subject = context.ClassNote.Where(x=>x.ClassNoteId.Equals(noteToSend.ClassNoteId)).Select(x=>x.Subject.Name).FirstOrDefault();
                    //var sessionIds = context.SessionClass.Where(x => noteToSend.Classes.Contains(x.ClassId.ToString())).Select(x=>x.SessionId).ToList();
                    //var userIds = context.StudentContact.Where(x=> sessionIds.Contains(x.SessionClassId)).Select(x=>x.UserId).ToList();
                    //var studentEmails = string.Join(",", context.Users.Where(x => userIds.Contains(x.Id)).Select(x => x.Email).ToList());

                    //await notificationService.CreateNotitficationAsync(new NotificationDTO
                    //{
                    //    Content = $"{subject} lesson note has been sent to you",
                    //    NotificationPageLink = $"smp-class/lesson-note-details?teacherClassNoteId={noteToSend.TeacherClassNoteId}",
                    //    NotificationSourceId = noteToSend.TeacherClassNoteId.ToString(),
                    //    Subject = "Class Note",
                    //    Receivers = studentEmails,
                    //    Type = "class-note",
                    //    ToGroup = "Students"
                    //});
                    //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
                    #endregion

                }
                else
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Sent to class(s) Successfully";
                res.Result = request;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        async Task<APIResponse<List<GetClasses2>>> IClassNoteService.GetStaffClassesOnNoteShareAsync(Guid teacherClassNoteId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var alreadyShared = context.TeacherClassNote.FirstOrDefault(d => d.ClientId == smsClientId && d.TeacherClassNoteId == teacherClassNoteId)?.Classes?.Split(',').ToList();
            var res = new APIResponse<List<GetClasses2>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClass.Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false)
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .OrderBy(s => s.Class.Name)
                        .Select(s => new GetClasses2(s, alreadyShared.Contains(s.ClassId.ToString()))).ToListAsync();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
                //GET TEACHER CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var classesAsASujectTeacher = context.SessionClass
                         .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false && e.SessionClassSubjects
                         .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)))
                         .Include(s => s.Class)
                         .Include(s => s.Session)
                         .Include(s => s.SessionClassSubjects)
                         .OrderBy(s => s.Class.Name);

                    var classesAsAFormTeacher = context.SessionClass
                        .Where(e => e.ClientId == smsClientId && e.Session.IsActive == true && e.Deleted == false && e.FormTeacherId == Guid.Parse(teacherId))
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name);

                    res.Result = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct()
                        .Select(s => new GetClasses2(s, alreadyShared is null ? false : alreadyShared.Contains(s.ClassId.ToString()))).GroupBy(x => x.ClassId).Select(x => x.First()).ToList();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PagedResponse<List<GetClassNotes>>>> IClassNoteService.GetMyWardsClassNotesByAsync(string subjectId, string studentContactId, PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetClassNotes>>>();

            var classId = context.StudentContact.Where(x => x.ClientId == smsClientId && x.StudentContactId == Guid.Parse(studentContactId))
                .Include(x => x.SessionClass).FirstOrDefault().SessionClass.ClassId.ToString();

            var query = context.TeacherClassNote
                        .Where(x => x.ClientId == smsClientId && x.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                        .Include(d => d.Teacher).ThenInclude(d => d.User)
                        .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                        .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                        .OrderByDescending(x => x.CreatedOn)
                        .Where(u => u.Deleted == false);

            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(u => Guid.Parse(subjectId) == u.ClassNote.SubjectId);
            }

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(u => u.Classes.Contains(classId));
            }

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetClassNotes(x, false)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }


        async Task<APIResponse<GetClassNotes>> IClassNoteService.GetSingleMyWardsClassNotesByAsync(Guid teacherClassNoteId)
        {
            var res = new APIResponse<GetClassNotes>();

            var query = context.TeacherClassNote
                .Where(x => x.ClientId == smsClientId && x.TeacherClassNoteId == teacherClassNoteId && x.Deleted == false)
               .Include(d => d.Teacher).ThenInclude(d => d.User)
                       .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                       .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                       .OrderByDescending(x => x.CreatedOn);

            res.Result = await query.Select(x => new GetClassNotes(x, true)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        public async Task<APIResponse<byte[]>> DownloadClassNotesByAsync(string classnoteId)
        {
            var res = new APIResponse<byte[]> ();
            try
            {
                var classNote = await context.ClassNote.FirstOrDefaultAsync(x => x.ClassNoteId == Guid.Parse(classnoteId));
                if(classNote == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Classnote doesn't exist!";
                    return res;
                }

                var document = new PdfDocument();
                string htmlContent = classNote.NoteContent;
                PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
                byte[] response;
                using(MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }
                res.Result = response;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        private async Task SendNotificationToAdminAsync(string userid, Guid classNoteId, Guid subjectId)
        {
            var appActivityId = "13f6763e-e5ab-45a0-8da1-244dbe63d298"; /// >>> Pls do not change Id //Can Review Lesson Notes
            var roleId = context.RoleActivity.FirstOrDefault(r => r.ActivityId == Guid.Parse(appActivityId) && r.Deleted == false && r.ClientId == smsClientId).RoleId;
            var userIds = context.UserRoles.Where(r => r.RoleId == roleId).Select(x => x.UserId).ToList();
            string adminsEmail = string.Join(",", context.Users.Where(a => a.Deleted == false && userIds.Contains(a.Id)).Select(x => x.Email).ToList());

            var subject = context.Subject.FirstOrDefault(m => m.SubjectId == subjectId && m.ClientId == smsClientId).Name;
            var author = context.Teacher.FirstOrDefault(m => m.UserId == userid && m.ClientId == smsClientId);
            await notificationService.CreateNotitficationAsync(new NotificationDTO
            {
                Content = $"{author.FirstName} {author.LastName} submitted note on {subject} for approval",
                NotificationPageLink = $"smp-notification/lesson-note-details?teacherClassNoteId={classNoteId}",
                NotificationSourceId = classNoteId.ToString(),
                Subject = "Class Note",
                ReceiversEmail = adminsEmail,
                Type = "class-note",
                ToGroup = "Admin"
            });
            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
        }
    
        private async Task NotifyTeacherOnNoteShareAsync(ClassNote noteToShare, ShareNotes request)
        {
            var teachers = context.Teacher.Where(x => x.ClientId == smsClientId && request.TeacherId.Contains(x.TeacherId)).Include(x => x.User).ToList();
            string teachersEmail = string.Join(",", teachers.Select(x => x.User.Email));

            await notificationService.CreateNotitficationAsync(new NotificationDTO
            {
                Content = $"Note with title {noteToShare.NoteTitle} has been shared with you",
                NotificationPageLink = $"smp-notification/lesson-note-details?teacherClassNoteId={noteToShare.ClassNoteId}",
                NotificationSourceId = noteToShare.ClassNoteId.ToString(),
                Subject = "Class Note",
                ReceiversEmail = teachersEmail,
                Type = "class-note",
                ToGroup = "Teachers"
            });
            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
        }

        private async Task NotifyTeacherOnNoteApprovalAsync(ClassNote note)
        {
            string teacherEmail = context.Users.FirstOrDefault(x => x.Id == note.Author).Email;
            var subject = context.Subject.FirstOrDefault(x => x.SubjectId == note.SubjectId && x.ClientId == smsClientId).Name;
            await notificationService.CreateNotitficationAsync(new NotificationDTO
            {
                Content = $"Note on {subject} has been approved",
                NotificationPageLink = $"smp-notification/lesson-note-details?ClassNoteId={note.ClassNoteId}",
                NotificationSourceId = note.ClassNoteId.ToString(),
                Subject = "Class Note Approved",
                ReceiversEmail = teacherEmail,
                Type = "class-note",
                ToGroup = "Teachers"
            });

            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
        }
    }
}
