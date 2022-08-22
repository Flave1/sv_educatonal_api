using BLL.MiddleWares;
using Contracts.Annoucements;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.Contracts.Assessment;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [AllowAnonymous]
    [Route("classassessment/api/v1")]
    public class ClassAssessmentController : Controller
    {
        private readonly IClassAssessmentService service;
        public ClassAssessmentController(IClassAssessmentService service)
        {
            this.service = service;
        }

        [HttpGet("get-all/class-assessments")]
        public async Task<IActionResult> GetStudentClassAssessmentsAsync()
        {
            var response = await service.GetAssessmentByTeacherAsync();
            return Ok(response);
        }

        [HttpPost("ceate/class-assessment")]
        public async Task<IActionResult> CreateClassAssessmentsAsync([FromBody] CreateClassAssessment request)
        {
            var response = await service.CreateClassAssessmentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-students/class-assessment")]
        public async Task<IActionResult> GetStudentClassAssessmentsAsync(string classAssessmentId)
        {
            var response = await service.GetClassStudentByAssessmentAsync(Guid.Parse(classAssessmentId));
            return Ok(response);
        }

        [HttpPost("update-student/class-assessment")]
        public async Task<IActionResult> UpdateStudentAssessmentScoreAsync([FromBody] UpdatetudentAssessmentScore request)
        {
            var response = await service.UpdateStudentAssessmentScoreAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/class-assessment/score")]
        public async Task<IActionResult> UpdateClassAssessmentScoreAsync([FromBody] UpdatClassAssessmentScore request)
        {
            var response = await service.UpdateClassAssessmentScoreAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
