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
    public interface IStudentNoteService
    {
        Task<APIResponse<StudentNotes>> CreateStudentNotesAsync(StudentNotes request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByTeachersAsync(string subjectId); 
        Task<APIResponse<List<GetStudentNotes>>> GetAllUnreviewedAsync();
        Task<APIResponse<UpdateStudentNote>> UpdateStudentNotesAsync(UpdateStudentNote request);
        Task<APIResponse<bool>> DeleteStudentNotesAsync(SingleDelete request);
        Task<APIResponse<bool>> ApproveOrDisapproveStudentNotesAsync(ApproveStudentNotes request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByAdminAsync();
        Task<APIResponse<List<GetStudentNotes>>> GetSingleStudentNotesByAdminAsync(SingleStudentNotes request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByStudentAsync();
        Task<APIResponse<List<GetStudentNotes>>> GetSingleStudentNotesByStudentAsync(SingleStudentNotes request);
        Task<APIResponse<GetStudentNotes>> GetSingleStudentNotesAsync(SingleStudentNotes request);
        Task<APIResponse<string>> AddCommentToStudentNoteAsync(Guid studentNoteId, string comment);
        Task<APIResponse<string>> ReplyStudentNoteCommentAsync(string comment, Guid commentId); 
        Task<APIResponse<List<StudentNoteComments>>> GetStudentNoteCommentsAsync(string studentNoteId);
    }
}
