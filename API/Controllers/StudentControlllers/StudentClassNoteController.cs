using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("smp/studentclassnotes/api/v1")]
    public class StudentClassNoteController : Controller
    {
        private readonly IStudentNoteService service;
        public StudentClassNoteController(IStudentNoteService service)
        {
            this.service = service;
        }

        [HttpGet("get-classnote/bystudents")]
        public async Task<IActionResult> GetStudentNoteCommentsAsync(string subjectId, int pageNumber, string termId)
        {
            PaginationFilter filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.filterClassNotesByStudentsAsync(subjectId, termId, filter);
            return Ok(response);
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
    }
}
