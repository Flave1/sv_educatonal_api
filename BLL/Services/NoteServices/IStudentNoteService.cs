using BLL;
using BLL.Filter;
using BLL.Wrappers;
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
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByTeachersAsync(string classId, string subjectId, int status); 
        Task<APIResponse<List<GetStudentNotes>>> GetAllUnreviewedAsync();
        Task<APIResponse<bool>> DeleteStudentNotesAsync(SingleDelete request);
        Task<APIResponse<bool>> ReviewStudentNoteAsync(ReviewStudentNoteRequest request);
        Task<APIResponse<List<GetStudentNotes>>> GetStudentNotesByStudentAsync(string subjectId, int status);
        Task<APIResponse<GetStudentNotes>> GetSingleStudentNotesAsync(Guid studentNoteId);
        Task<APIResponse<string>> AddCommentToStudentNoteAsync(Guid studentNoteId, string comment);
        Task<APIResponse<string>> AddCommentToClassNoteAsync(Guid guid, string comment);
        Task<APIResponse<string>> ReplyStudentNoteCommentAsync(string comment, Guid commentId); 
        Task<APIResponse<List<StudentNoteComments>>> GetStudentNoteCommentsAsync(string studentNoteId);
        Task<APIResponse<PagedResponse<List<GetClassNotes>>>> filterClassNotesByStudentsAsync(string subjectId, PaginationFilter filter);
        Task<APIResponse<GetStudentNotes>> GetSingleStudentNotesAsync(string studentNoteId);
        Task<APIResponse<SendStudentNote>> SendStudentNoteForReviewAsync(SendStudentNote request);
        Task<APIResponse<string>> ReplyClassNoteCommentAsync(string comment, Guid commentId);
        Task<APIResponse<PagedResponse<List<GetStudentNotes>>>> GetWardNotesAsync(string subjectId, string studentContactId, string classId, PaginationFilter filter);
        Task<APIResponse<GetStudentNotes>> GetSingleWardNotesAsync(Guid StudentNoteId);
    }
}
