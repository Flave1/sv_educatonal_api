using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Authentication;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Common;
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
        private readonly IPaginationService paginationService;

        public StudentNoteService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
        }

        async Task<APIResponse<StudentNotes>> IStudentNoteService.CreateStudentNotesAsync(StudentNotes request)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var studentContact = context.StudentContact.FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));
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
                };

                await context.StudentNote.AddAsync(newStudentNote);
                await context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.Result = request;
            }
            catch (Exception ex)
            {

                throw;
            }
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
         
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetStudentNotesByTeachersAsync(string classId, string subjectId, int status)
        {
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var res = new APIResponse<List<GetStudentNotes>>();

            var query = context.StudentNote
                        .Include(s => s.Student).ThenInclude(s => s.User)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher).ThenInclude(d => d.User)
                        .Include(d => d.Subject).Where(u => u.Deleted == false && u.SessionClassId == Guid.Parse(classId) && u.SessionClass.Session.IsActive == true && u.AprrovalStatus != (int)NoteApprovalStatus.Saved);

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
            res.Result = await query.Select(x => new GetStudentNotes(x)).ToListAsync();
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetAllUnreviewedAsync()
        {
            var res = new APIResponse<List<GetStudentNotes>>();
            var noteList = await context.StudentNote
                    .Include(e => e.Subject)
                    .Include(e => e.Student).ThenInclude(e => e.User)
                    .Include(e => e.SessionClass).ThenInclude(s => s.Session)
                    .Where(u => u.Deleted == false
                         && u.SessionClass.Session.IsActive == true
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
            var note = await context.StudentNote.FirstOrDefaultAsync(d => d.Deleted == false 
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

            var studentNote = context.StudentNote.FirstOrDefault(d => d.StudentNoteId == Guid.Parse(request.StudentNoteId));
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

            var studentNote = context.StudentNote.FirstOrDefault(d => d.StudentNoteId == Guid.Parse(request.StudentNoteId));
            if (studentNote is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            else
            {
                studentNote.AprrovalStatus = (int)NoteApprovalStatus.InProgress;
                await context.SaveChangesAsync();
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully submited for review";
            res.Result = request;
            return res;

        }
        async Task<APIResponse<GetStudentNotes>> IStudentNoteService.GetSingleStudentNotesAsync(Guid studentNoteId)
        {
            var res = new APIResponse<GetStudentNotes>();
            res.Result = await context.StudentNote
                .Include(e => e.Subject)
                .Include(e => e.Student).ThenInclude(e => e.User)
                .Include(e => e.SessionClass)
                .Where(u => u.Deleted == false 
                && u.StudentNoteId == studentNoteId) 
                .Select(x => new GetStudentNotes(x)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
        async Task<APIResponse<List<GetStudentNotes>>> IStudentNoteService.GetStudentNotesByStudentAsync(string subjectId, int status)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var res = new APIResponse<List<GetStudentNotes>>();
            if (!string.IsNullOrEmpty(studentContactId))
            {
                var query = context.StudentNote
                         .Include(s => s.Student).ThenInclude(s => s.User)
                         .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                         .Include(d => d.Teacher).ThenInclude(d => d.User)
                         .Include(d => d.Subject)
                         .OrderByDescending(x => x.CreatedOn)
                          .Where(u => u.Deleted == false
                          && u.SessionClass.Session.IsActive == true
                          && u.StudentContactId == Guid.Parse(studentContactId));

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

                res.Result = await query.Select(x => new GetStudentNotes(x)).ToListAsync();



                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess; 
            }
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
                StudentNote = note,
                UserId = userid
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
                RepliedToId = commentId,
                UserId = userid
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
            var res = new APIResponse<List<StudentNoteComments>>();

            res.Result = await context.StudentNoteComment
                .Include(s => s.StudentNote)
                .Include(s => s.User)
                .Include(d => d.Replies).ThenInclude(d => d.RepliedTo)
                .Include(d => d.Replies).ThenInclude(s => s.User)
                .Include(d => d.Replies).ThenInclude(s => s.User)
                .Include(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(s => s.User)
                .Where(u => u.Deleted == false && u.StudentNoteId == Guid.Parse(studentNoteId) && u.IsParent == true)
                .Select(x => new StudentNoteComments(x)).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

 
        async Task<APIResponse<PagedResponse<List<GetClassNotes>>>> IStudentNoteService.filterClassNotesByStudentsAsync(string subjectId, PaginationFilter filter)
        {
            var studentContactId = accessor.HttpContext.User.FindFirst(e => e.Type == "studentContactId")?.Value;
            var studentClass = context.StudentContact.Include(x => x.SessionClass).FirstOrDefault(d => d.StudentContactId == Guid.Parse(studentContactId));

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
                    var query = context.TeacherClassNote
                            .Include(d => d.Teacher).ThenInclude(d => d.User)
                            .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                            .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                            .OrderBy(d => d.CreatedBy)
                         .Where(u => u.Deleted == false
                         && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved
                         && u.ClassNote.SubjectId == Guid.Parse(subjectId));

                    
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
                    var query = context.TeacherClassNote
                             .Include(d => d.Teacher).ThenInclude(d => d.User)
                             .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                             .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                             .OrderBy(d => d.CreatedBy)
                          .Where(u => u.Deleted == false
                          && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Approved);

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
            res.Result = await context.StudentNote
                        .Include(s => s.Student).ThenInclude(s => s.User)
                        .Include(s => s.SessionClass).ThenInclude(s => s.Session)
                        .Include(d => d.Teacher).ThenInclude(d => d.User)
                        .Include(d => d.Subject)
                         .Where(u => u.Deleted == false
                         && u.SessionClass.Session.IsActive == true
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
                var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == classNoteId);
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
            }
            catch (Exception ex)
            {
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
            var note = await context.TeacherClassNoteComment.FirstOrDefaultAsync(d => d.TeacherClassNoteCommentId == commentId);
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

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

    }
}
