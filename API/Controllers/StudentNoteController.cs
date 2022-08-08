using BLL.MiddleWares; 
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [AllowAnonymous]
    [Route("studentnotes/api/v1")]
    public class StudentNoteController : Controller
    {
        private readonly IStudentNoteService service;
        public StudentNoteController(IStudentNoteService service)
        {
            this.service = service;
        }

        [HttpPost("create/studentnote")]
        public async Task<IActionResult> CreateStudentNotesAsync([FromBody] StudentNotes request)
        {
            var response = await service.CreateStudentNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/studentnote")]
        public async Task<IActionResult> UpdateStudentNotesAsync([FromBody] UpdateStudentNote request)
        {
            var response = await service.UpdateStudentNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/studentnotes/by-teacher")]
        public async Task<IActionResult> GetStudentNotesByTeachersAsync(string subjectId)
        {
            var response = await service.GetStudentNotesByTeachersAsync(subjectId);
            return Ok(response);
        }
          
        [HttpGet("get/single/Studentnotes/by-admin")]
        public async Task<IActionResult> GetSingleStudentNotesByAdminAsync(SingleStudentNotes request)
        {
            var response = await service.GetSingleStudentNotesByAdminAsync(request);
            return Ok(response);
        }
        [HttpGet("get/single/Studentnotes/by-student")]
        public async Task<IActionResult> GetSingleStudentNotesByStudentAsync(SingleStudentNotes request)
        {
            var response = await service.GetSingleStudentNotesByStudentAsync(request);
            return Ok(response);
        }

        [HttpGet("get/Studentnotes/by-admin")]
        public async Task<IActionResult> GetStudentNotesByAdminAsync()
        {
            var response = await service.GetStudentNotesByAdminAsync();
            return Ok(response);
        }

        [HttpGet("get/Studentnotes/by-student")]
        public async Task<IActionResult> GetStudentNotesByStudentAsync()
        {
            var response = await service.GetStudentNotesByStudentAsync();
            return Ok(response);
        }

        [HttpGet("get/not-approved/studentnote")]
        public async Task<IActionResult> GetAllApprovalInProgressNoteAsync()
        {
            var response = await service.GetAllApprovalInProgressNoteAsync();
            return Ok(response);
        }

        [HttpPost("approve-or-dissaprove/studentnote")]
        public async Task<IActionResult> ApproveOrDisapproveStudentNotesAsync([FromBody] ApproveStudentNotes request)
        {
            var response = await service.ApproveOrDisapproveStudentNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
  
         

        [HttpPost("delete/studentnote")]
        public async Task<IActionResult> DeleteStudentNotesAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteStudentNotesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
