using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Notes;
using SMP.DAL.Models.NoteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NoteServices
{
    public class ClassNoteService : IClassNoteService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;

        public ClassNoteService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
        }

        async Task<APIResponse<ClassNotes>> IClassNoteService.CreateClassNotesAsync(ClassNotes request)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var termid = context.SessionTerm.FirstOrDefault(x => x.IsActive).SessionTermId;
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
                    Classes = string.Join(',', request.Classes)
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
            if (status == -2)
            {
                query = context.TeacherClassNote
               .Include(d => d.Teacher).ThenInclude(d => d.User)
                       .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                       .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                       .OrderByDescending(x => x.CreatedOn)
                    .Where(u => u.Deleted == false && u.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress);

                if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
                }

                if (!string.IsNullOrEmpty(termId))
                {
                    query = query.Where(d => d.ClassNote.SessionTermId == Guid.Parse(termId));
                }

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetClassNotes(x, false)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            }
            else
            {
                query = context.TeacherClassNote
               .Include(d => d.Teacher).ThenInclude(d => d.User)
                       .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                       .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                       .OrderByDescending(x => x.CreatedOn)
                    .Where(u => u.Deleted == false);

                if (!accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH) && !accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    query = query.Where(x => x.TeacherId == Guid.Parse(teacherId));
                }

                if (!string.IsNullOrEmpty(subjectId))
                {
                    query = query.Where(u => Guid.Parse(subjectId) == u.ClassNote.SubjectId);
                }
                if (status >= 0)
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

            }


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
                    res.Result = await context.TeacherClassNote
                    .Include(d => d.Teacher).ThenInclude(d => d.User)
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
                    res.Result = await context.TeacherClassNote
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
                         && u.AprrovalStatus == (int)NoteApprovalStatus.Approved && u.Author == userid)
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
                            TeacherId = teacher
                        };
                        await context.TeacherClassNote.AddAsync(newClassNote);

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

            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<GetClassNotes>> IClassNoteService.GetSingleTeacherClassNotesAsync(string TeacherClassNoteId)
        {
            var res = new APIResponse<GetClassNotes>();
            res.Result = await context.TeacherClassNote  
                .Include(d => d.Teacher).ThenInclude(d => d.User)
                .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail).ThenInclude(x => x.Teacher)
                .Where(u => u.Deleted == false 
                && u.TeacherClassNoteId == Guid.Parse(TeacherClassNoteId)) 
                .Select(x => new GetClassNotes(x, true)).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetSingleClassNotesByAdminAsync(SingleClassNotes request)
        {
            var res = new APIResponse<List<GetClassNotes>>();
            res.Result = await context.ClassNote
                .Include(d => d.Subject)         
                .Where(u => u.Deleted == false && u.AprrovalStatus == (int)NoteApprovalStatus.Approved)
                .Select(x => new GetClassNotes(x)).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<List<ClassNoteTeachers>>> IClassNoteService.GetTeachersNoteSharedToAsync(Guid classNoteId)
        {
            var res = new APIResponse<List<ClassNoteTeachers>>();

            var alreadyShared = context.ClassNote.Include(x => x.TeacherClassNotes).Where(d => d.ClassNoteId == classNoteId).SelectMany(s => s.TeacherClassNotes).Select(x => x.TeacherId).ToList();
            res.Result = await context.Teacher.OrderByDescending(d => d.CreatedOn).Include(s => s.User)
                 .Where(d => d.Deleted == false  && d.Status == (int)TeacherStatus.Active).Select(a => new ClassNoteTeachers(a, alreadyShared.Contains(a.TeacherId))).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<ClassNoteTeachers>>> IClassNoteService.GetOtherTeachersAsync(Guid classNoteId)
        {
            var res = new APIResponse<List<ClassNoteTeachers>>();

            var alreadyShared = context.ClassNote.Include(x => x.TeacherClassNotes).Where(d => d.ClassNoteId == classNoteId).SelectMany(s => s.TeacherClassNotes).Select(x => x.TeacherId).ToList();
            res.Result = await context.Teacher.OrderByDescending(d => d.CreatedOn).Include(s => s.User)
                 .Where(d => d.Deleted == false && d.Status == (int)TeacherStatus.Active && alreadyShared.Contains(d.TeacherId)).Select(a => new ClassNoteTeachers(a)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.SendClassNoteForAprovalAsync(Guid classNoteId)
        {
            var res = new APIResponse<string>();

            var note = await context.ClassNote.FirstOrDefaultAsync(d => d.ClassNoteId == classNoteId);
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

            res.Message.FriendlyMessage = "Successfully sent for approval";
            res.IsSuccessful = true;
            res.Result = classNoteId.ToString();
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.AddCommentToClassNoteAsync(Guid classNoteId, string comment)
        {
            var res = new APIResponse<string>();

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
                UserId = userId,
            };

            context.TeacherClassNoteComment.Add(commented);
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = "Comment sent";
            res.IsSuccessful = true;
            res.Result = comment;
            return res;
        }

        async Task<APIResponse<string>> IClassNoteService.ReplyClassNoteCommentAsync(string comment, Guid commentId)
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

        async Task<APIResponse<List<ClassNoteComment>>> IClassNoteService.GetClassNoteCommentsAsync(string classNoteId)
        {
            var res = new APIResponse<List<ClassNoteComment>>();
            res.Result = await context.TeacherClassNoteComment
                .Include(x => x.AppUser)
                .Include(d => d.Replies).ThenInclude(d => d.RepliedTo)
                .Include(d => d.Replies).ThenInclude(s => s.AppUser)
                .Include(d => d.Replies).ThenInclude(d => d.Replies).ThenInclude(x => x.AppUser)
                .Where(u => u.Deleted == false && u.ClassNoteId == Guid.Parse(classNoteId) && u.IsParent == true)
                .Select(x => new ClassNoteComment(x)).ToListAsync();

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }



        async Task<APIResponse<List<GetClassNotes>>> IClassNoteService.GetRelatedClassNoteAsync(Guid classNoteId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherid = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            var note = context.ClassNote.FirstOrDefault(d => d.ClassNoteId == classNoteId);

            var res = new APIResponse<List<GetClassNotes>>();
            if (!string.IsNullOrEmpty(teacherid))
            {
                res.Result = await context.TeacherClassNote
                 .Include(d => d.Teacher).ThenInclude(d => d.User)
                         .Include(x => x.ClassNote).ThenInclude(x => x.Subject)
                         .Include(x => x.ClassNote).ThenInclude(d => d.AuthorDetail)
                          .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.Saved)
                           .OrderBy(d => d.ClassNote.AprrovalStatus == (int)NoteApprovalStatus.InProgress)
                      .Where(u => u.Deleted == false && u.ClassNote.SubjectId == note.SubjectId
                         && u.ClassNote.AprrovalStatus != (int)NoteApprovalStatus.Saved 
                         && u.ClassNote.AprrovalStatus != (int)NoteApprovalStatus.NotApproved  
                         && u.TeacherId == Guid.Parse(teacherid))
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
                var noteToSend = context.TeacherClassNote.Include(d => d.ClassNote).FirstOrDefault(x => x.TeacherClassNoteId == Guid.Parse(request.TeacherClassNoteId));
                if (noteToSend is not null)
                {
                    if(noteToSend.ClassNote.AprrovalStatus is not (int)NoteApprovalStatus.Approved)
                    {
                        res.Message.FriendlyMessage = "Unapproved note cannot be sent to students";
                        return res;
                    }
                    noteToSend.Classes = string.Join(',', request.Classes);

                    await context.SaveChangesAsync();
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
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        

        async Task<APIResponse<List<GetClasses2>>> IClassNoteService.GetStaffClassesOnNoteShareAsync(Guid teacherClassNoteId)
        {
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var alreadyShared = context.TeacherClassNote.FirstOrDefault(d => d.TeacherClassNoteId == teacherClassNoteId)?.Classes?.Split(',').ToList();
            var res = new APIResponse<List<GetClasses2>>();

            if (!string.IsNullOrEmpty(userid))
            {
                //GET SUPER ADMIN CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    res.Result = await context.SessionClass
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false).Select(s => new GetClasses2(s, alreadyShared.Contains(s.ClassId.ToString()))).ToListAsync();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }
                //GET TEACHER CLASSES
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var classesAsASujectTeacher = context.SessionClass
                         .Include(s => s.Class)
                         .Include(s => s.Session)
                         .Include(s => s.SessionClassSubjects)
                         .OrderBy(s => s.Class.Name)
                         .Where(e => e.Session.IsActive == true && e.Deleted == false && e.SessionClassSubjects
                         .Any(d => d.SubjectTeacherId == Guid.Parse(teacherId)));

                    var classesAsAFormTeacher = context.SessionClass
                        .Include(s => s.Class)
                        .Include(s => s.Session)
                        .Include(s => s.SessionClassSubjects)
                        .OrderBy(s => s.Class.Name)
                        .Where(e => e.Session.IsActive == true && e.Deleted == false && e.FormTeacherId == Guid.Parse(teacherId));


                    res.Result = classesAsASujectTeacher.ToList().Concat(classesAsAFormTeacher.ToList()).Distinct().Select(s => new GetClasses2(s, alreadyShared is null ? false : alreadyShared.Contains(s.SessionClassId.ToString()))).ToList();
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.IsSuccessful = true;
                    return res;
                }

            }
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

    }
}
