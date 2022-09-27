using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BLL.Services.SubjectServices;
using Contracts.Class;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("subject/api/v1/")]
    public class SubjectController : Controller
    { 
        private readonly ISubjectService service; 
        public SubjectController(ISubjectService service)
        {
            this.service = service;
        }


        #region subject S

        [HttpPost("create/subject")]
        public async Task<IActionResult> CreateSubjectAsync([FromBody] ApplicationLookupCommand request)
        {
            var response = await service.CreateSubjectAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/subject")]
        public async Task<IActionResult> UpdateSubjectAsync([FromBody] ApplicationLookupCommand request)
        {
            var response = await service.UpdateSubjectAsync(request.Name, request.LookupId, request.IsActive);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/subject")]
        public async Task<IActionResult> GetAllSubjectsAsync()
        {
            var response = await service.GetAllSubjectsAsync();
            return Ok(response);
        }


        [HttpGet("getall/active-subject")]
        public async Task<IActionResult> GetAllActiveSubjectsAsync()
        {
            var response = await service.GetAllActiveSubjectsAsync();
            return Ok(response);
        }


        [HttpPost("delete/subject")]
        public async Task<IActionResult> DeleteSubjectAsync([FromBody] MultipleDelete reguest)
        {
            var response = await service.DeleteSubjectAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/student-subjects")]
        public IActionResult GetAllStudentSubjects()
        {
            var response = service.GetAllStudentSubjects();
            return Ok(response);
        }

        [HttpGet("get/subject-teacher")]
        public IActionResult GetSubjectTer(Guid subjectId)
        {
            var response = service.GetSubjectTeacher(subjectId);
            return Ok(response);
        }

        #endregion


    }
} 