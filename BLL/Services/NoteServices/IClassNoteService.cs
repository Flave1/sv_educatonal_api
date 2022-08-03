using BLL;
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
        Task<APIResponse<List<GetClassNotes>>> GetClassNotesByTeachersAsync(); 
        Task<APIResponse<List<GetClassNotes>>> GetAllApprovalInProgressNoteAsync();
        Task<APIResponse<UpdateClassNote>> UpdateClassNotesAsync(UpdateClassNote request);
        Task<APIResponse<bool>> DeleteClassNotesByAdminAsync(SingleDelete request);
        Task<APIResponse<bool>> ApproveOrDisapproveClassNotesAsync(ApproveClassNotes request);
        Task<APIResponse<ShareNotes>> ShareClassNotesAsync(ShareNotes request);
        Task<APIResponse<bool>> DeleteTeacherClassNotesAsync(SingleDelete request);
    }
}
