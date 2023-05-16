using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SMP.API.Hubs;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.NotififcationServices;
using SMP.BLL.Services.SessionServices;
using SMP.Contracts.Notes;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NoteServices
{
    public class StudentNoteService : IStudentNoteService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly IHubContext<NotificationHub> hub;
        private readonly INotificationService notificationService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly ITermService termService;

        public StudentNoteService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService,
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

        async Task<APIResponse<StudentNotes>> IStudentNoteService.CreateStudentNotesAsync(StudentNotes request)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var studentContact = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId) && d.ClientId == smsClientId);
            var termId = termService.GetCurrentTerm().SessionTermId;
            var res = new APIResponse<StudentNotes>();

            try
            {
                var newStudentNote = new StudentNote()
                {
                    NoteTitle = request.NoteTitle,
                    NoteContent = request.NoteContent,
                    AprrovalStatus = request.SubmitForReview ? (int)NoteApprovalStatus.InProgress : (int)NoteApprovalStatus.Saved,
                    StudentContactId = Guid.Parse(studentContactId),
                    SubjectId = Guid.Parse(request.SubjectId),
                    TeacherId = Guid.Parse(request.TeacherId),
                    SessionClassId = studentContact.SessionClassId,
                    SessionTermId = termId,
                };
                await context.StudentNote.AddAsync(newStudentNote);
                await context.SaveChangesAsync();

                #region notification
                //if(request.SubmitForReview)
                //{
                //    var studentName = context.Users.FirstOrDefault(x=>x.Id == studentContact.UserId).FirstName;
                //    var classId = context.SessionClass.FirstOrDefault(x=>x.SessionClassId == studentContact.SessionClassId).ClassId;
                //    var className  = context.ClassLookUp.FirstOrDefault(x=>x.ClassLookupId == classId).Name;
                //    var subject = context.Subject.FirstOrDefault(m => m.SubjectId == Guid.Parse(request.SubjectId)).Name;
                //    var userId = context.Teacher.FirstOrDefault(x => x.TeacherId == Guid.Parse(request.TeacherId)).UserId;
                //    var teacherEmail = context.Users.FirstOrDefault(x=>x.Id == userId).FirstName;

                //    await notificationService.CreateNotitficationAsync(new NotificationDTO
                //    {
                //        Content = $"{studentName} in {className} submitted {subject} note",
                //        NotificationPageLink = $"dashboard/smp-notification/lesson-note-details?teacherClassNoteId={newStudentNote.StudentNoteId}",
                //        NotificationSourceId = newStudentNote.StudentNoteId.ToString(),
                //        Subject = "Student Note",
                //        ReceiversEmail = teacherEmail,
                //        Type = "student-note",
                //        ToGroup = "Teachers"
                //    });
                //    await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());

                //}
                #endregion

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.Result = request;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
            return res;
        }

        async Task<APIResponse<bool>> IStudentNoteService.DeleteStudentNotesAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var note = await context.StudentNote.FirstOrDefaultAsync(x=>x.StudentNoteId == Guid.Parse(request.Item) && x.ClientId == smsClientId);
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
         
        async Task<APIResponse<PagedResponse<List<GetStudentNotes>>>> IStudentNoteService.GetStudentNotesByTeachersAsync(string classId, string subjectId, int status, PaginationFilter filter)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<PagedResponse<List<GetStudentNotes>>>();

            var query = context.StudentNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Student)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher)
                        .Include(d => d.Subject).Where(u => u.SessionClassId == Guid.Parse(classId) && u.SessionClass.Session.IsActive == true && u.AprrovalStatus != (int)NoteApprovalStatus.Saved);

            if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
            {
                query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
            }
           
            if (status >= 0)
            {
                query = query.Where(u => u.AprrovalStatus == status);
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                if (subjectId != "all")
                {
                    query = query.Where(u => u.SubjectId == Guid.Parse(subjectId));
                }
            }


            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetStudentNotes(x)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<PagedResponse<List<GetStudentNotes>>>> IStudentNoteService.GetStudentNotesByTeachers2Async(string classId, string subjectId, int status, PaginationFilter filter)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<PagedResponse<List<GetStudentNotes>>>();

            var query = context.StudentNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Student)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher)
                        .Include(d => d.Subject).Where(u => u.SessionClassId == Guid.Parse(classId) && u.SessionClass.Session.IsActive == true && u.AprrovalStatus != (int)NoteApprovalStatus.Saved);

            if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
            {
                query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
            }

            if (status >= 0)
            {
                query = query.Where(u => u.AprrovalStatus == status);
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                if (subjectId != "all")
                {
                    query = query.Where(u => u.SubjectId == Guid.Parse(subjectId));
                }
            }


            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetStudentNotes(x)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }


        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetAllUnreviewedAsync()
        {
            var res = new APIResponse<List<GetStudentNotes>>();
            var noteList = await context.StudentNote.Where(x=> x.ClientId == smsClientId && x.Deleted == false)
                    .Include(e => e.Subject)
                    .Include(e => e.Student)
                    .Include(e => e.SessionClass).ThenInclude(s => s.Session)
                    .Where(u => u.SessionClass.Session.IsActive == true
                    && u.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                    .Select(x => new GetStudentNotes(x)).ToListAsync();

            res.Result = noteList;

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<bool>> IStudentNoteService.ReviewStudentNoteAsync(ReviewStudentNoteRequest request)
        { 
            var res = new APIResponse<bool>();
            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.ClientId == smsClientId && d.Deleted == false 
            && d.StudentNoteId == Guid.Parse(request.StudentNoteId));
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
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var res = new APIResponse<UpdateStudentNote>();

            var studentNote = context.StudentNote.FirstOrDefault(d => d.ClientId == smsClientId && d.StudentNoteId == Guid.Parse(request.StudentNoteId));
            if(studentNote is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            else
            {
                studentNote.NoteTitle = request.NoteTitle;
                studentNote.NoteContent = request.NoteContent;
                studentNote.SubjectId = Guid.Parse(request.SubjectId);
                studentNote.TeacherId = Guid.Parse(request.TeacherId);
                studentNote.AprrovalStatus = request.SubmitForReview ? (int)NoteApprovalStatus.InProgress : studentNote.AprrovalStatus;
                await context.SaveChangesAsync();
            }


            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            return res;

        }

        async Task<APIResponse<SendStudentNote>> IStudentNoteService.SendStudentNoteForReviewAsync(SendStudentNote request)
        {
            var res = new APIResponse<SendStudentNote>();

            var studentNote = context.StudentNote.FirstOrDefault(d => d.ClientId == smsClientId && d.StudentNoteId == Guid.Parse(request.StudentNoteId));
            if (studentNote is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            else
            {
                studentNote.AprrovalStatus = (int)NoteApprovalStatus.InProgress;
                await context.SaveChangesAsync();

                #region notification
                //var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
                //var studentName = context.Users.FirstOrDefault(x => x.Id == userId).FirstName;
                //var studentClass = context.StudentContact.Where(x=>x.UserId.Equals(userId)).Select(x=>x.SessionClass.Class.Name).FirstOrDefault();
                //var subject = context.Subject.FirstOrDefault(x=>x.SubjectId == studentNote.SubjectId).Name;
                //var teacherId = context.Teacher.FirstOrDefault(x => x.TeacherId == studentNote.TeacherId).UserId;
                //var teacherEmail = context.Users.Where(x=>x.Id == teacherId).FirstOrDefault().Email;

                //await notificationService.CreateNotitficationAsync(new NotificationDTO
                //{
                //    Content = $"{studentName} in {studentClass} submitted {subject} note",
                //    NotificationPageLink = $"smp-class/lesson-note-details?StudentNoteId={studentNote.StudentNoteId}",
                //    NotificationSourceId = studentNote.StudentNoteId.ToString(),
                //    Subject = "Student Note",
                //    Receivers = teacherEmail,
                //    Type = "student-note",
                //    ToGroup = "Teachers"
                //});
                //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
                #endregion
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully submited for review";
            res.Result = request;
            return res;

        }
        async Task<APIResponse<GetStudentNotes>> IStudentNoteService.GetSingleStudentNotesAsync(Guid studentNoteId)
        {
            var res = new APIResponse<GetStudentNotes>();
            res.Result = await context.StudentNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                .Include(e => e.Subject)
                .Include(e => e.Student)
                .Include(e => e.SessionClass)
                .Where(u => u.StudentNoteId == studentNoteId) 
                .Select(x => new GetStudentNotes(x)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<PagedResponse<List<GetStudentNotes>>>> IStudentNoteService.GetStudentNotesByStudentAsync(string subjectId, int status, string termId, PaginationFilter filter)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var res = new APIResponse<PagedResponse<List<GetStudentNotes>>>();
            if (!string.IsNullOrEmpty(studentContactId))
            {
                var query = context.StudentNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                         .Include(s => s.Student)
                         .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                         .Include(d => d.Teacher)
                         .Include(d => d.Subject)
                         .OrderByDescending(x => x.CreatedOn)
                          .Where(u => u.SessionClass.Session.IsActive == true
                          && u.StudentContactId == Guid.Parse(studentContactId));

                if (!string.IsNullOrEmpty(termId))
                {
                    query = query.Where(u => u.SessionTermId == Guid.Parse(termId));
                }

                if (!string.IsNullOrEmpty(subjectId))
                {
                    query = query.Where(u => u.SubjectId == Guid.Parse(subjectId));
                }

                if (status  == -2)
                {
                    query = query.Where(u => u.AprrovalStatus == (int)NoteApprovalStatus.Approved || u.AprrovalStatus == (int)NoteApprovalStatus.InProgress || u.AprrovalStatus == (int)NoteApprovalStatus.Saved).Take(50);
                }
                else 
                {
                    query = query.Where(u => u.AprrovalStatus == status);
                }

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetStudentNotes(x)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess; 
            }
            return res;
        }


        async Task<APIResponse<string>> IStudentNoteService.AddCommentToStudentNoteAsync(Guid studentNoteId, string comment)
        {
            var res = new APIResponse<string>();

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.ClientId == smsClientId && d.StudentNoteId == studentNoteId);
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
                StudentNote = note,
                UserId = userid
            };

            context.StudentNoteComment.Add(commented);
            await context.SaveChangesAsync();

            #region notification
            //var commenter = context.Users.FirstOrDefault(m => m.Id == userid).FirstName;
            //var subject = context.Subject.FirstOrDefault(m => m.SubjectId == note.SubjectId).Name;

            //string receiverEmail = "";
            //string toGroup = "";
            //if (teacherId != null)
            //{
            //    receiverEmail = context.Users.FirstOrDefault(x => x.Id == userid).Email;
            //    toGroup = "Students";
            //}
            //else
            //{
            //    var teacherUserId = context.Teacher.FirstOrDefault(x => x.TeacherId == Guid.Parse(teacherId)).UserId;
            //    receiverEmail = context.Users.FirstOrDefault(x => x.Id == teacherUserId).Email;
            //    toGroup = "Teachers";
            //}

            //await notificationService.CreateNotitficationAsync(new NotificationDTO
            //{
            //    Content = $"{commenter} commented on {subject} note",
            //    NotificationPageLink = $"smp-class/lesson-note-details?StudentNoteId={note.StudentNoteId}",
            //    NotificationSourceId = note.StudentNoteId.ToString(),
            //    Subject = "Student Note",
            //    Receivers = receiverEmail,
            //    Type = "student-note",
            //    ToGroup = toGroup
            //});
            //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
            #endregion

            res.Message.FriendlyMessage = "Comment sent succesfully";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<string>> IStudentNoteService.ReplyStudentNoteCommentAsync(string comment, Guid commentId)
        {
            var res = new APIResponse<string>();

            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = await context.StudentNoteComment.FirstOrDefaultAsync(d => d.ClientId == smsClientId && d.StudentNoteCommentId == commentId);
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
                RepliedToId = commentId,
                UserId = userid
            };

            context.StudentNoteComment.Add(commented);
            await context.SaveChangesAsync();

            #region notification
            //var commenter = context.Users.FirstOrDefault(m => m.Id == userid).FirstName;
            //var subjectId = context.StudentNote.FirstOrDefault(m => m.StudentNoteId == note.StudentNoteId).SubjectId;
            //var subject = context.Subject.FirstOrDefault(m => m.SubjectId == subjectId).Name;
            //string receiverEmail = context.Users.FirstOrDefault(x => x.Id == note.UserId).Email;

            //string toGroup = "";
            //if (teacherId != null)
            //{
            //    toGroup = "Student";
            //}
            //else
            //{
            //    toGroup = "Teachers";
            //}

            //await notificationService.CreateNotitficationAsync(new NotificationDTO
            //{
            //    Content = $"{commenter} commented on {subject} note",
            //    NotificationPageLink = $"smp-class/lesson-note-details?StudentNoteId={note.StudentNoteId}",
            //    NotificationSourceId = note.StudentNoteId.ToString(),
            //    Subject = "Student Note",
            //    Receivers = receiverEmail,
            //    Type = "student-note",
            //    ToGroup = toGroup
            //});
            //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
            #endregion

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<List<StudentNoteComments>>> IStudentNoteService.GetStudentNoteCommentsAsync(string studentNoteId)
        {
            var res = new APIResponse<List<StudentNoteComments>>();

            res.Result = await context.StudentNoteComment.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                .Include(s => s.StudentNote)
                .Include(s => s.User)
                .Include(d => d.Replies).ThenInclude(d => d.RepliedTo)
                .Include(d => d.Replies)
                .Include(d => d.Replies)
                .Include(d => d.Replies).ThenInclude(d => d.Replies)
                .Where(u => u.StudentNoteId == Guid.Parse(studentNoteId) && u.IsParent == true)
                .Select(x => new StudentNoteComments(x, 
                    context.Teacher.Where(c => c.ClientId == smsClientId && c.UserId == x.UserId).Select(d => new { FirstName = d.FirstName, LastName = d.LastName }).FirstOrDefault() ??
                    context.StudentContact.Where(c => c.ClientId == smsClientId && c.UserId == x.UserId).Select(d => new { FirstName = d.FirstName, LastName = d.LastName }).FirstOrDefault())).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res; 
        }

 
        async Task<APIResponse<PagedResponse<List<GetClassNotes>>>> IStudentNoteService.filterClassNotesByStudentsAsync(string subjectId, string termId, PaginationFilter filter)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var studentClass = context.StudentContact.Where(x => x.ClientId == smsClientId).Include(x => x.SessionClass).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));

            var res = new APIResponse<PagedResponse<List<GetClassNotes>>>();
            if(studentClass is null)
            {
                return new APIResponse<PagedResponse<List<GetClassNotes>>>();
            }
            if (!string.IsNullOrEmpty(studentContactId))
            {
                var classId = studentClass.SessionClass.ClassId.ToString();
                if (!string.IsNullOrEmpty(subjectId))
                {
                    var query = context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                            .Include(d => d.Teacher).ThenInclude(d => d.User)
                            .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                            .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                            .OrderBy(d => d.CreatedBy)
                         .Where(u => u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved
                         && u.ClassNote.SubjectId == Guid.Parse(subjectId));

                    if (!string.IsNullOrEmpty(classId))
                    {
                        query = query.Where(x => x.SessionTermId == Guid.Parse(termId));
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

                }
                else
                {
                    var query = context.TeacherClassNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                             .Include(d => d.Teacher).ThenInclude(d => d.User)
                             .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                             .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                             .OrderBy(d => d.CreatedBy)
                             .Where(u => u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved);

                    if (!string.IsNullOrEmpty(classId))
                    {
                        query = query.Where(x => x.SessionTermId == Guid.Parse(termId));
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

                }
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return await Task.Run(() => res);
        }

        async Task<APIResponse<GetStudentNotes>> IStudentNoteService.GetSingleStudentNotesAsync(string studentNoteId)
        {
            var res = new APIResponse<GetStudentNotes>();
            res.Result = await context.StudentNote.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                        .Include(s => s.Student)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher)
                        .Include(d => d.Subject)
                         .Where(u => u.SessionClass.Session.IsActive == true
                         && u.StudentNoteId == Guid.Parse(studentNoteId))
                         .Select(x => new GetStudentNotes(x)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<string>> IStudentNoteService.AddCommentToClassNoteAsync(Guid classNoteId, string comment)
        {
            var res = new APIResponse<string>();

            try
            {

                var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
                var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

                var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClientId == smsClientId && d.ClassNoteId == classNoteId);
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
                    UserId = userId
                };

                context.TeacherClassNoteComment.Add(commented);
                await context.SaveChangesAsync();

                #region notification
                //var commenter = context.Users.FirstOrDefault(m => m.Id == userId).FirstName;
                //var subject = context.Subject.FirstOrDefault(m => m.SubjectId == note.SubjectId).Name;

                //string receiverEmail = "";
                //string toGroup = "";
                //if(teacherId != null)
                //{
                //    receiverEmail = context.Users.FirstOrDefault(x => x.Id == userId).Email;
                //    toGroup = "Students";
                //}
                //else
                //{
                //    var teacherUserId = context.Teacher.FirstOrDefault(x => x.TeacherId == Guid.Parse(teacherId)).UserId;
                //    receiverEmail = context.Users.FirstOrDefault(x => x.Id == teacherUserId).Email;
                //    toGroup = "Teachers";
                //}

                //await notificationService.CreateNotitficationAsync(new NotificationDTO
                //{
                //    Content = $"{commenter} commented on {subject} note",
                //    NotificationPageLink = $"smp-class/lesson-note-details?ClassNoteId={note.ClassNoteId}",
                //    NotificationSourceId = note.ClassNoteId.ToString(),
                //    Subject = "Class Note",
                //    Receivers = receiverEmail,
                //    Type = "class-note",
                //    ToGroup = toGroup
                //});
                //await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
                #endregion
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw ex;
            }

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<string>> IStudentNoteService.ReplyClassNoteCommentAsync(string comment, Guid commentId)
        {
            var res = new APIResponse<string>();

            var userId = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = await context.TeacherClassNoteComment.FirstOrDefaultAsync(d => d.ClientId == smsClientId && d.TeacherClassNoteCommentId == commentId);
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

            //string toGroup = "";
            //if (teacherId != null)
            //{
            //    toGroup = "Student";
            //}
            //else
            //{
            //    toGroup = "Teachers";
            //}

            //await notificationService.CreateNotitficationAsync(new NotificationDTO
            //{
            //    Content = $"{commenter} commented on {subject} note",
            //    NotificationPageLink = $"smp-class/lesson-note-details?StudentNoteId={note.ClassNoteId}",
            //    NotificationSourceId = note.ClassNoteId.ToString(),
            //    Subject = "Class Note",
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

        async Task<APIResponse<PagedResponse<List<GetStudentNotes>>>> IStudentNoteService.GetWardNotesAsync(string subjectId, string classId, string studentContactId, PaginationFilter filter)
        {
            var studentClass = context.StudentContact.Include(x => x.SessionClass).FirstOrDefault(d => d.ClientId == smsClientId && d.StudentContactId == Guid.Parse(studentContactId));

            var res = new APIResponse<PagedResponse<List<GetStudentNotes>>>();
            if (studentClass is null)
            {
                return new APIResponse<PagedResponse<List<GetStudentNotes>>>();
            }
            if (!string.IsNullOrEmpty(studentContactId))
            {
                // 
                var query = context.StudentNote
                         .Where(u => u.ClientId == smsClientId && u.Deleted == false && u.StudentContactId == Guid.Parse(studentContactId) && u.SessionClassId == Guid.Parse(classId))
                         .Include(s => s.Student)
                         .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                         .Include(d => d.Teacher)
                         .Include(d => d.Subject)
                         .OrderByDescending(x => x.CreatedOn)
                         .Where(u => u.SessionClass.Session.IsActive == true && u.AprrovalStatus == (int)NoteApprovalStatus.Approved);

                if (!string.IsNullOrEmpty(subjectId))
                {
                    query = query.Where(u => u.SubjectId == Guid.Parse(subjectId));
                }

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetStudentNotes(x)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return await Task.Run(() => res);
        }


        async Task<APIResponse<GetStudentNotes>> IStudentNoteService.GetSingleWardNotesAsync(Guid StudentNoteId)
        {
            var res = new APIResponse<GetStudentNotes>();

            var query = context.StudentNote
                        .Where(u => u.ClientId == smsClientId && u.StudentNoteId == StudentNoteId && u.Deleted == false)
                        .Include(s => s.Student)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher)
                        .Include(d => d.Subject)
                        .OrderByDescending(x => x.CreatedOn)
                        .Where(u => u.SessionClass.Session.IsActive == true && u.AprrovalStatus == (int)NoteApprovalStatus.Approved);

            res.Result = await query.Select(x => new GetStudentNotes(x)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return await Task.Run(() => res);
        }

    }
}
