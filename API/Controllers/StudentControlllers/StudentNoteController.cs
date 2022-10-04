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
    [Route("smp/studentnotes/api/v1")]
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
        public async Task<IActionResult> GetStudentNotesByTeachersAsync(string classId, string subjectId, int status)
        {
            var response = await service.GetStudentNotesByTeachersAsync(classId, subjectId, status);
            return Ok(response);
        }
        

        [HttpGet("get/studentnotes/by-student")]
        public async Task<IActionResult> GetStudentNotesByStudentAsync(string subjectId, int status)
        {
            var response = await service.GetStudentNotesByStudentAsync(subjectId, status);
            return Ok(response);
        }
       

        [HttpGet("get/unreviewed/studentnote")]
        public async Task<IActionResult> GetAllUnreviewedAsync()
        {
            var response = await service.GetAllUnreviewedAsync();
            return Ok(response);
        }

        [HttpPost("review/studentnote")]
        public async Task<IActionResult> ReviewStudentNoteAsync([FromBody] ReviewStudentNoteRequest request)
        {
            var response = await service.ReviewStudentNoteAsync(request);
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


        [HttpPost("add-comment/to-studentnote")]
        public async Task<IActionResult> AddCommentToStudentNoteAsync([FromBody] AddCommentToStudentNote request)
        {
            var response = await service.AddCommentToStudentNoteAsync(Guid.Parse(request.StudentNoteId), request.Comment);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("reply/studentnote-comment")]
        public async Task<IActionResult> ReplyStudentNoteCommentAsync([FromBody] ReplyCommentToStudentNote request)
        {
            var response = await service.ReplyStudentNoteCommentAsync(request.Comment, Guid.Parse(request.CommentId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-studentnote/comments")]
        public async Task<IActionResult> GetStudentNoteCommentsAsync(string studentNoteId)
        {
            var response = await service.GetStudentNoteCommentsAsync(studentNoteId);
            return Ok(response);
        }

        [HttpGet("get-single/studentnote")]
        public async Task<IActionResult> GetSingleStudentNotesAsync(string studentNoteId)
        {
            var response = await service.GetSingleStudentNotesAsync(studentNoteId);
            return Ok(response);
        }

        [HttpPost("send/studentnote-forreview")]
        public async Task<IActionResult> SendStudentNoteForReviewAsync([FromBody] SendStudentNote request)
        {
            var response = await service.SendStudentNoteForReviewAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
