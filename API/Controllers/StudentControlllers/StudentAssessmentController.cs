using BLL.MiddleWares; 
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Assessment;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("studentassessment/api/v1")]
    public class StudentAssessmentController : Controller
    {
        private readonly IHomeAssessmentService service;
        public StudentAssessmentController(IHomeAssessmentService service)
        {
            this.service = service;
        }

        [HttpGet("get/open-assessments")]
        public async Task<IActionResult> GetHomeAssessmentsByStudentAsync()
        {
            var response = await service.GetHomeAssessmentsByStudentAsync();
            return Ok(response);
        }
        [HttpPost("submit/assessment-feedback")]
        public async Task<IActionResult> SubmitHomeAssessmentByStudentAsync([FromBody] CreateHomeAssessmentFeedback request)
        {
            var response = await service.SubmitHomeAssessmentByStudentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
