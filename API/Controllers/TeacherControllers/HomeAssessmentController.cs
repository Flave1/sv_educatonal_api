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
    [Route("homeassessment/api/v1")]
    public  class HomeAssessmentController : Controller
    {
        private readonly IHomeAssessmentService service;
        public HomeAssessmentController(IHomeAssessmentService service)
        {
            this.service = service;
        }

        [HttpPost("create/home-assessment")]
        public async Task<IActionResult> CreateHomeAssessmentsAsync([FromBody] CreateHomeAssessmentRequest request)
        {
            var response = await service.CreateHomeAssessmentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/home-assessment")]
        public async Task<IActionResult> UpdateHomeAssessmentsAsync([FromBody] UpdateHomeAssessmentRequest request)
        {
            var response = await service.UpdateHomeAssessmentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("get/home-assessments")]
        public async Task<IActionResult> GetHomeAssessmentsAsync(string sessionClassId, string sessionClassSubjectId, string groupId)
        {
            var response = await service.GetSubjectHomeAssessmentAsync(sessionClassId, sessionClassSubjectId, groupId);
            return Ok(response);
        }

        [HttpGet("get/single/home-assessments")]
        public async Task<IActionResult> GetSingleHomeAssessmentsAsync(string homeassessmentId, string sessionClassid)
        {
            var response = await service.GetSingleHomeAssessmentAsync(Guid.Parse(homeassessmentId), sessionClassid);
            return Ok(response);
        }

        [HttpPost("delete/home-assessment")]
        public async Task<IActionResult> DeleteHomeAssessmentsAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteHomeAssessmentAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("send/home-assessment")]
        public async Task<IActionResult> SendHomeAssessmentsAsync([FromBody] SendHomeAssessmentRequest request)
        {
            var response = await service.SendHomeAssessmentToStudentsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/subject/assessment-score")]
        public async Task<IActionResult> GetSubjectAssessmentScoreRecordAsync(string sessionClassSubjectId, string sessionClassid)
        {
            var response = await service.GetSubjectAssessmentScoreRecordAsync(Guid.Parse(sessionClassSubjectId), Guid.Parse(sessionClassid));
            return Ok(response);
        }

        [HttpPost("score/assessment-feedback")]
        public async Task<IActionResult> ScoreHomeAssessmentsAsync([FromBody] ScoreHomeAssessmentFeedback request)
        {
            var response = await service.ScoreHomeAssessmentByStudentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
