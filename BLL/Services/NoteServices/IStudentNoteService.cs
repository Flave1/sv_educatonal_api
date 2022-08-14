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
        Task<APIResponse<UpdateStudentNote>> UpdateStudentNotesAsync(UpdateStudentNote request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByTeachersAsync(string subjectId); 
        Task<APIResponse<List<GetStudentNotes>>> GetAllUnreviewedAsync();
        Task<APIResponse<bool>> DeleteStudentNotesAsync(SingleDelete request);
        Task<APIResponse<bool>> ReviewStudentNoteAsync(ReviewStudentNoteRequest request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByStudentAsync(string subjectId);
        Task<APIResponse<GetStudentNotes>> GetSingleStudentNotesAsync(Guid studentNoteId);
        Task<APIResponse<string>> AddCommentToStudentNoteAsync(Guid studentNoteId, string comment);
        Task<APIResponse<string>> ReplyStudentNoteCommentAsync(string comment, Guid commentId); 
        Task<APIResponse<List<StudentNoteComments>>> GetStudentNoteCommentsAsync(string studentNoteId);
    }
}
