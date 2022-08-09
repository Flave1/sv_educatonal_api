using BLL;
using Contracts.Authentication;
using Contracts.Common;
using SMP.Contracts.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NoteServices
{
    public interface IClassNoteService
    {
        Task<APIResponse<ClassNotes>> CreateClassNotesAsync(ClassNotes request);
        Task<APIResponse<List<GetClassNotes>>> GetClassNotesByTeachersAsync(string subjectId); 
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
    }
}
