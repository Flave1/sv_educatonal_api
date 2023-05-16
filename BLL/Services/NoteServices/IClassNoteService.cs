using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using SMP.Contracts.Notes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NoteServices
{
    public interface IClassNoteService
    {
        Task<APIResponse<ClassNotes>> CreateClassNotesAsync(ClassNotes request);
        Task<APIResponse<PagedResponse<List<GetClassNotes>>>> GetClassNotesByTeachersAsync(string classId, string subjectId, int status, string termId, PaginationFilter filter); 
        Task<APIResponse<GetClassNotes>> GetSingleTeacherClassNotesAsync(string TeacherClassNoteId); 
        Task<APIResponse<List<GetClassNotes>>> GetAllApprovalInProgressNoteAsync();
        Task<APIResponse<UpdateClassNote>> UpdateClassNotesAsync(UpdateClassNote request);
        Task<APIResponse<bool>> DeleteClassNotesByAdminAsync(SingleDelete request);
        Task<APIResponse<bool>> ApproveOrDisapproveClassNotesAsync(ApproveClassNotes request);
        Task<APIResponse<ShareNotes>> ShareClassNotesAsync(ShareNotes request);
        Task<APIResponse<bool>> DeleteTeacherClassNotesAsync(SingleDelete request);
        Task<APIResponse<List<GetClassNotes>>> GetClassNotesByAdminAsync();
        Task<APIResponse<List<GetClassNotes>>> GetSingleClassNotesByAdminAsync(SingleClassNotes request);
        Task<APIResponse<List<ClassNoteTeachers>>> GetTeachersNoteSharedToAsync(Guid classNoteId);
        Task<APIResponse<string>> SendClassNoteForAprovalAsync(Guid classNoteId);
        Task<APIResponse<string>> AddCommentToClassNoteAsync(Guid classNoteId, string comment);
        Task<APIResponse<string>> ReplyClassNoteCommentAsync(string comment, Guid commentId);
        Task<APIResponse<List<ClassNoteComment>>> GetClassNoteCommentsAsync(string classNoteId);
        Task<APIResponse<List<GetClassNotes>>> GetClassNotesByStatusAsync(string subjectId, int status);
        Task<APIResponse<List<ClassNoteTeachers>>> GetOtherTeachersAsync(Guid classNoteId);
        Task<APIResponse<List<GetClassNotes>>> GetRelatedClassNoteAsync(Guid classNoteId);
        Task<APIResponse<SendNote>> SendClassNoteToClassesAsync(SendNote request);
        Task<APIResponse<List<GetClasses2>>> GetStaffClassesOnNoteShareAsync(Guid teacherClassNoteId);
        Task<APIResponse<PagedResponse<List<GetClassNotes>>>> GetMyWardsClassNotesByAsync(string subjectId, string studentContactId, PaginationFilter filter);
        Task<APIResponse<GetClassNotes>> GetSingleMyWardsClassNotesByAsync(Guid teacherClassNoteId);
        Task<APIResponse<byte[]>> DownloadClassNotesByAsync(string classnoteId);
        Task<APIResponse<PagedResponse<List<GetClassNotes>>>> GetClassNotesByTeachersMobileAsync(string classId, string subjectId, int status, PaginationFilter filter);
    }
}
