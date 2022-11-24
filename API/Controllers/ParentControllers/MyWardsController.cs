using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.BLL.Services.NoteServices;
using SMP.BLL.Services.ParentServices;
using System;
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
        private readonly IHomeAssessmentService homeAssessmentService;
        public MyWardsController(IParentService service, IStudentNoteService studentNoteService, IClassNoteService classNoteService, IHomeAssessmentService homeAssessmentService)
        {
            this.service = service;
            this.studentNoteService = studentNoteService;
            this.classNoteService = classNoteService;
            this.homeAssessmentService = homeAssessmentService;
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

        [HttpGet("get-single/maywards-notes")]
        public async Task<IActionResult> GetSingleMyWardsNoteAsync(Guid studentNoteId)
        {
            var response = await studentNoteService.GetSingleStudentNotesAsync(studentNoteId);
            return Ok(response);
        }

        [HttpGet("get-single/maywards-class-notes")]
        public async Task<IActionResult> GetSingleMyWardsClassNoteAsync(string teacherClassNoteId)
        {
            var response = await classNoteService.GetSingleTeacherClassNotesAsync(teacherClassNoteId);
            return Ok(response);
        }

        [HttpGet("get/maywards-home-assessments")]
        public async Task<IActionResult> GetWardsWardHomeAssessmentAsync(int pageNumber, Guid sessionClassSubjectId, string studentContactId, int status)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await homeAssessmentService.FilterHomeAssessmentsByParentAsync(sessionClassSubjectId, status, studentContactId, filter);
            return Ok(response);
        }
    }
}
