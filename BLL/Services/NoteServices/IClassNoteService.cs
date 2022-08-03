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
        Task<APIResponse<List<GetClassNotes>>> GetClassNotesAsync(); 
        Task<APIResponse<List<GetClassNotes>>> GetAllNotApprovedClassNotesAsync();
        Task<APIResponse<UpdateClassNote>> UpdateClassNotesAsync(UpdateClassNote request);
        Task<APIResponse<bool>> DeleteClassNotesAsync(SingleDelete request);
        Task<APIResponse<bool>> ApproveOrDisapproveClassNotesAsync(ApproveClassNotes request);
        Task<APIResponse<ShareNotes>> ShareClassNotesAsync(ShareNotes request);
    }
}
