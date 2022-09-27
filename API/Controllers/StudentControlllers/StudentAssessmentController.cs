using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.Contracts.Assessment;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("smp/studentassessment/api/v1")]
    public class StudentAssessmentController : Controller
    {
        private readonly IHomeAssessmentService service;
        public StudentAssessmentController(IHomeAssessmentService service)
        {
            this.service = service;
        }

        [Obsolete]
        [HttpGet("get/open-assessments")]
        public async Task<IActionResult> GetHomeAssessmentsByStudentAsync()
        {
            var response = await service.GetHomeAssessmentsByStudentAsync();
            return Ok(response);
        }

        [HttpGet("filter/home-assessments")]
        public async Task<IActionResult> FilterHomeAssessmentsByStudentAsync(int status)
        {
            var response = await service.FilterHomeAssessmentsByStudentAsync(status);
            return Ok(response);
        }

        [HttpGet("get-single/home-assessments")]
        public async Task<IActionResult> GetSingleHomeAssessmentsByStudentAsync(string homeAssessmentFeedBackId)
        {
            var response = await service.GetSingleHomeAssessmentsByStudentAsync(Guid.Parse(homeAssessmentFeedBackId));
            return Ok(response);
        }
        [HttpPost("submit/assessment-feedback")]
        public async Task<IActionResult> SubmitHomeAssessmentByStudentAsync([FromForm] CreateHomeAssessmentFeedback request)
        {
            var response = await service.SubmitHomeAssessmentByStudentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
