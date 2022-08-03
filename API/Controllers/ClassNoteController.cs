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

        [HttpPost("approve-or-dissaprove/classnote")]
        public async Task<IActionResult> ApproveOrDisapproveClassNotesAsync([FromBody] ApproveClassNotes request)
        {
            var response = await service.ApproveOrDisapproveClassNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
          
        [HttpGet("get/classnotes")]
        public async Task<IActionResult> GetClassNotesAsync()
        {
            var response = await service.GetClassNotesAsync();
            return Ok(response);
        }

        [HttpPost("delete/classnotes")]
        public async Task<IActionResult> DeleteClassNotesAsync([FromBody]SingleDelete request)
        {
            var response = await service.DeleteClassNotesAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
