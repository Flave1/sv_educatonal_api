using BLL.MiddleWares; 
using Contracts.Common;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Notes;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("classnotes/api/v1")]
    public class ClassNoteController : Controller
    {
        private readonly IClassNoteService service;
        public ClassNoteController(IClassNoteService service)
        {
            this.service = service;
        }

        [HttpPost("create/classnote")]
        public async Task<IActionResult> CreateClassNotesAsync([FromBody] ClassNotes request)
        {
            var response = await service.CreateClassNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/classnote")]
        public async Task<IActionResult> UpdateClassNotesAsync([FromBody] UpdateClassNote request)
        {
            var response = await service.UpdateClassNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("share/classnote")]
        public async Task<IActionResult> ShareClassNotesAsync([FromBody] ShareNotes request)
        {
            var response = await service.ShareClassNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/classnotes/by-teacher")]
        public async Task<IActionResult> GetClassNotesByTeachersAsync()
        {
            var response = await service.GetClassNotesByTeachersAsync();
            return Ok(response);
        }

        [HttpGet("get/single/classnotes/by-teacher")]
        public async Task<IActionResult> GetSingleClassNotesByTeachersAsync(SingleTeacherClassNotes request)
        {
            var response = await service.GetSingleClassNotesByTeachersAsync(request);
            return Ok(response);
        }

        [HttpGet("get/single/classnotes/by-admin")]
        public async Task<IActionResult> GetSingleClassNotesByAdminAsync(SingleClassNotes request)
        {
            var response = await service.GetSingleClassNotesByAdminAsync(request);
            return Ok(response);
        }

        [HttpGet("get/classnotes/by-admin")]
        public async Task<IActionResult> GetClassNotesByAdminAsync()
        {
            var response = await service.GetClassNotesByAdminAsync();
            return Ok(response);
        }

        [HttpGet("get/not-approved/classnotes")]
        public async Task<IActionResult> GetAllApprovalInProgressNoteAsync()
        {
            var response = await service.GetAllApprovalInProgressNoteAsync();
            return Ok(response);
        }

        [HttpPost("approve-or-dissaprove/classnote")]
        public async Task<IActionResult> ApproveOrDisapproveClassNotesAsync([FromBody] ApproveClassNotes request)
        {
            var response = await service.ApproveOrDisapproveClassNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
  

        [HttpPost("delete/teacher/classnotes")]
        public async Task<IActionResult> DeleteTeacherClassNotesAsync([FromBody]SingleDelete request)
        {
            var response = await service.DeleteTeacherClassNotesAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("delete/classnotes")]
        public async Task<IActionResult> DeleteClassNotesByAdminAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteClassNotesByAdminAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
