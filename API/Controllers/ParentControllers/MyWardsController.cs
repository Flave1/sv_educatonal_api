using BLL.Filter;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Annoucements;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.BLL.Services.NoteServices;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.TimetableServices;
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
        private readonly IStudentService studentService;
        private readonly ITimeTableService timeTableService;
        public MyWardsController(IParentService service, IStudentNoteService studentNoteService, IClassNoteService classNoteService, IHomeAssessmentService homeAssessmentService, ITimeTableService timeTableService, IStudentService studentService)
        {
            this.service = service;
            this.studentNoteService = studentNoteService;
            this.classNoteService = classNoteService;
            this.homeAssessmentService = homeAssessmentService;
            this.timeTableService = timeTableService;
            this.studentService = studentService;
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
        public async Task<IActionResult> GetMyWardsClassNoteAsync(int pageNumber, string studentId, string subjectId)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await classNoteService.GetMyWardsClassNotesByAsync(subjectId, studentId, filter);
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


        [HttpGet("get-maywards/class-timetable")]
        public async Task<IActionResult> GetSingleMyWardsClassNoteAsync(Guid classlkpId)
        {
            var response = await timeTableService.GetClassTimeTableByParentsAsync(classlkpId);
            return Ok(response);
        }

        [HttpGet("get-single/{StudentAccountId}")]
        public async Task<IActionResult> GetSingleStudentsAsync(string StudentAccountId)
        {
            var response = await studentService.GetSingleStudentAsync(Guid.Parse(StudentAccountId));
            return Ok(response);
        }

        [HttpGet("get/maywards-announcements")]
        public async Task<IActionResult> GetMyWardsAnnouncementsAsync(PaginationFilter filter)
        {
            var response = await service.GetAnnouncementsAsync(filter);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/maywards-announcement-details/{announcementId}")]
        public async Task<IActionResult> GetMyWardsAnnouncementDetailsAsync(string announcementId)
        {
            var response = await service.GetAnnouncementDetailsAsync(announcementId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("get/maywards/update/seen-announcement")]
        public async Task<IActionResult> UpdateSeenAnnouncementAsync([FromBody]UpdatSeenAnnouncement request)
        {
            var response = await service.UpdateSeenAnnouncementAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/dashboard-count")]
        public async Task<IActionResult> GetDashboardCountAsync()
        {
            var response = await service.GetDashboardCount();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/profile/{parentId}")]
        public async Task<IActionResult> GetParentByIdAsync(string parentId)
        {
            var response = await service.GetParentByIdAsync(parentId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/parent-wards")]
        public async Task<IActionResult> GetParentWardsAsync(PaginationFilter filter, string parentId)
        {
            var response = await service.GetParentWardsAsync(filter, parentId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
