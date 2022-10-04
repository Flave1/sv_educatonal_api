using BLL.Helpers;
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
        public async Task<IActionResult> GetClassNotesByTeachersAsync(string classId, string subjectId, int status, string termId)
        {
            var response = await service.GetClassNotesByTeachersAsync(classId, subjectId, status, termId);
            return Ok(response);
        }

        [HttpGet("get/single/teacher-classnote")]
        public async Task<IActionResult> GetSingleTeacherClassNotesAsync(string TeacherClassNoteId)
        {
            var response = await service.GetSingleTeacherClassNotesAsync(TeacherClassNoteId);
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

        [HttpGet("get-note/other-teachers")]
        public async Task<IActionResult> GetTeachersNoteSharedToAsync(string classNoteId)
        {
            var response = await service.GetTeachersNoteSharedToAsync(Guid.Parse(classNoteId));
            return Ok(response);
        }

        [HttpPost("send/classnotes/for-approval")]
        public async Task<IActionResult> SendClassNoteForAprovalAsync([FromBody] SingleClassNotes request)
        {
            var response = await service.SendClassNoteForAprovalAsync(Guid.Parse(request.ClassNoteId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("add-comment/to-classnote")]
        public async Task<IActionResult> AddCommentToClassNoteAsync([FromBody] AddCommentToClassNote request)
        {
            var response = await service.AddCommentToClassNoteAsync(Guid.Parse(request.ClassNoteId), request.Comment);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("reply/classnote-comment")]
        public async Task<IActionResult> ReplyClassNoteCommentAsync([FromBody] ReplyCommentToClassNote request)
        {
            var response = await service.ReplyClassNoteCommentAsync(request.Comment, Guid.Parse(request.CommentId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-classnote/comments")]
        public async Task<IActionResult> GetClassNoteCommentsAsync(string classNoteId)
        {
            var response = await service.GetClassNoteCommentsAsync(classNoteId);
            return Ok(response);
        }

        [HttpGet("get/teacher-classnote/by-status")]
        public async Task<IActionResult> GetSingleTeacherClassNotesAsync(string subjectId, int status)
        {
            var response = await service.GetClassNotesByStatusAsync(subjectId, status);
            return Ok(response);
        }

        [HttpGet("get/classnote-viewers")]
        public async Task<IActionResult> GetOtherTeachersAsync(string classNoteId)
        {
            var response = await service.GetOtherTeachersAsync(Guid.Parse(classNoteId));
            return Ok(response);
        }



        [HttpGet("get/related-classnote")]
        public async Task<IActionResult> GetRelatedClassNoteAsync(string classNoteId)
        {
            var response = await service.GetRelatedClassNoteAsync(Guid.Parse(classNoteId));
            return Ok(response);
        }

        [HttpPost("send/classnotes/to-students")]
        public async Task<IActionResult> SendClassNoteToClassesAsync([FromBody] SendNote request)
        {
            var response = await service.SendClassNoteToClassesAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-note/shared-class")]
        public async Task<IActionResult> GetStaffClassesOnNoteShareAsync(string teacherClassNoteId)
        {
            var response = await service.GetStaffClassesOnNoteShareAsync(Guid.Parse(teacherClassNoteId));
            return Ok(response);
        }

    }
}
