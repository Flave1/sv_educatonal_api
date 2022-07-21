using API.Controllers.BaseControllers;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PinManagementService;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.PinManagement;
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
        private readonly IPinManagementService pinService;
        public ResultsController(IResultsService service, IPinManagementService pinService)
        {
            this.service = service;
            this.pinService = pinService;
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

        [HttpGet("get/master-list")]
        public async Task<IActionResult> GetMasterListAsync(string sessionClassid, string termId)
        {
            var response = await service.GetMasterListAsync(Guid.Parse(sessionClassid), Guid.Parse(termId));
            return Ok(response);
        }
        [HttpGet("get/cumulative-master-list")]
        public async Task<IActionResult> GetCumulativeMasterListAsync(string sessionClassid, string termId)
        {
            var response = await service.GetCumulativeMasterListAsync(Guid.Parse(sessionClassid), Guid.Parse(termId));
            return Ok(response);
        }
        [HttpGet("get/result-list")]
        public async Task<IActionResult> GetListOfResultsAsync(string sessionClassid, string termId)
        {
            var response = await service.GetListOfResultsAsync(Guid.Parse(sessionClassid), Guid.Parse(termId));
            return Ok(response);
        }

        [HttpGet("get/single-student/result-entries")]
        public async Task<IActionResult> GetSingleStudentScoreEntryAsync(string sessionClassid, string termId, string studentContactId)
        {
            var response = await service.GetSingleStudentScoreEntryAsync(Guid.Parse(sessionClassid), Guid.Parse(termId), Guid.Parse(studentContactId));
            return Ok(response);
        }

        [HttpPost("update/publish-result")]
        public async Task<IActionResult> PublishResultAsync([FromBody] PublishResultRequest request)
        {
            var response = await service.PublishResultAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("get/previous-terms/class-score-entries/{sessionClassid}")]
        public async Task<IActionResult> GetPreviousTermsClassSubjectScoreEntriesAsync(string sessionClassid, string subjectId, string sessionTermId)
        {
            var response = await service.GetPreviousTermsClassSubjectScoreEntriesAsync(Guid.Parse(sessionClassid), Guid.Parse(subjectId), Guid.Parse(sessionTermId));
            return Ok(response);
        }

        [HttpPost("update/previous-terms/exam-score")]
        public async Task<IActionResult> UpdatePreviousTermsExamScore([FromBody] UpdateOtherSessionScore request)
        {
            var response = await service.UpdatePreviousTermsExamScore(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/previous-terms/assessment-score")]
        public async Task<IActionResult> UpdatePreviousTermsAssessmentScore([FromBody] UpdateOtherSessionScore request)
        {
            var response = await service.UpdatePreviousTermsAssessmentScore(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/previous-terms/preview-class/score-entries")]
        public async Task<IActionResult> PreviewPreviousTermsClassScoreEntry(string sessionClassid, string subjectId, string sessionTermId)
        {
            var response = await service.PreviewPreviousTermsClassScoreEntry(Guid.Parse(sessionClassid), Guid.Parse(subjectId), Guid.Parse(sessionTermId));
            return Ok(response);
        }

        [HttpGet("get/student-result")]
        public async Task<IActionResult> GetStudentResultAsync(string sessionClassid, string termId, string studentContactId)
        {
            var response = await service.GetStudentResultAsync(Guid.Parse(sessionClassid), Guid.Parse(termId), Guid.Parse(studentContactId));
            return Ok(response);
        }

        [HttpPost("print/result")]
        public async Task<IActionResult> PrintResultAsync([FromBody] PrintResultRequest request)
        {
            var response = await pinService.PrintResultAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
} 