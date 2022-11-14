using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.NoteServices;
using SMP.BLL.Services.ParentServices;
using System.Threading.Tasks;

namespace SMP.API.Controllers.ParentControllers
{
    [PortalAuthorize]
    [Route("smp/maywards/api/v1")]
    public class MyWardsController : Controller
    {
        private readonly IParentService service;
        private readonly IStudentNoteService studentNoteService;
        private readonly IClassNoteService classNoteService;
        public MyWardsController(IParentService service, IStudentNoteService studentNoteService, IClassNoteService classNoteService)
        {
            this.service = service;
            this.studentNoteService = studentNoteService;
            this.classNoteService = classNoteService;
        }

        [HttpGet("get/maywards")]
        public async Task<IActionResult> GetMyWardsAsync(int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetMyWardsAsync(filter);
            return Ok(response);
        }


        [HttpGet("get/maywards-notes")]
        public async Task<IActionResult> GetMyWardsNoteAsync(int pageNumber, string classId, string subjectId, string studentContactId)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await studentNoteService.GetWardNotesAsync(subjectId, classId, studentContactId, filter);
            return Ok(response);
        }

        [HttpGet("get/maywards-class-notes")]
        public async Task<IActionResult> GetMyWardsClassNoteAsync(int pageNumber, string classId, string subjectId)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await classNoteService.GetMyWardsClassNotesByAsync(subjectId, classId, filter);
            return Ok(response);
        }

    }
}
