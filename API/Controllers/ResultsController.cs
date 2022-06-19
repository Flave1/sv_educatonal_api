using API.Controllers.BaseControllers;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.ResultModels;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("api/v1/result/")]
    public class ResultsController : BaseController
    {
        private readonly IResultsService service;
        public ResultsController(IResultsService service)
        {
            this.service = service;
        }

        [HttpGet("get/staff-classes")]
        public async Task<IActionResult> GetCurrentStaffClassesAsync()
        {
            var response = await service.GetCurrentStaffClassesAsync();
            return Ok(response);
        }

        [HttpGet("get/staff-class-subjects/{sessionClassid}")]
        public async Task<IActionResult> GetCurrentStaffClassSubjectsAsync(string sessionClassid)
        {
            var response = await service.GetCurrentStaffClassSubjectsAsync(Guid.Parse(sessionClassid));
            return Ok(response);
        }

        [HttpGet("get/class-score-entries/{sessionClassid}")]
        public async Task<IActionResult> GetClassSubjectScoreEntriesAsync(string sessionClassid, string subjectId)
        {
            var response = await service.GetClassEntryAsync(Guid.Parse(sessionClassid), Guid.Parse(subjectId));
            return Ok(response);
        }

        [HttpPost("update/exam-score")]
        public async Task<IActionResult> UpdateExamScore([FromBody] UpdateScore request)
        {
            var response = await service.UpdateExamScore(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/assessment-score")]
        public async Task<IActionResult> UpdateAssessmentScore([FromBody] UpdateScore request)
        {
            var response = await service.UpdateAssessmentScore(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/preview-class/score-entries")]
        public async Task<IActionResult> PreviewClassScoreEntry(string sessionClassid, string subjectId)
        {
            var response = await service.PreviewClassScoreEntry(Guid.Parse(sessionClassid), Guid.Parse(subjectId));
            return Ok(response);
        }
    }
} 