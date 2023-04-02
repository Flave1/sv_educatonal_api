using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ResultServices;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [PortalAuthorize]
    [Route("api/v1/result/")]
    public class ResultsController : Controller
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
        [Obsolete]
        [HttpGet("get/formteacher-classes")]
        public async Task<IActionResult> GetFormTeacherClassesAsync()
        {
            var response = await service.GetFormTeacherClassesAsync();
            return Ok(response);
        }

        [HttpGet("get/staff-class-subjects/{sessionClassid}")]
        public async Task<IActionResult> GetCurrentStaffClassSubjectsAsync(string sessionClassid)
        {
            var response = await service.GetCurrentStaffClassSubjectsAsync(Guid.Parse(sessionClassid));
            return Ok(response);
        }
        [HttpGet("get/staff-class-subjects/by-classlookup/{classId}/{sessionClassId}")]
        public async Task<IActionResult> GetCurrentStaffClassSubjects2Async(string classId, string sessionClassId)
        {
            var response = await service.GetCurrentStaffClassSubjects2Async(Guid.Parse(classId), Guid.Parse(sessionClassId));
            return Ok(response);
        }
       
        [HttpGet("get/class-score-entries/{sessionClassid}")]
        public async Task<IActionResult> GetClassSubjectScoreEntriesAsync(string sessionClassid, string subjectId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber, PageSize = 50 };
            var response = await service.GetClassEntryAsync(Guid.Parse(sessionClassid), Guid.Parse(subjectId), filter);
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
        public async Task<IActionResult> PreviewClassScoreEntry(string sessionClassid, string subjectId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.PreviewClassScoreEntry(Guid.Parse(sessionClassid), Guid.Parse(subjectId), filter);
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
        public async Task<IActionResult> GetClassResultListAsync(string sessionClassid, string termId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = 1 };
            var response = await service.GetClassResultListAsync(Guid.Parse(sessionClassid), Guid.Parse(termId), filter);
            return Ok(response);
        }

        [HttpGet("get/single-student/result-entries")]
        public async Task<IActionResult> GetSingleStudentScoreEntryAsync(string sessionClassid, string termId, string studentContactId)
        {
            var response = await service.GetSingleStudentScoreEntryAsync(Guid.Parse(sessionClassid), Guid.Parse(termId), Guid.Parse(studentContactId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
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
        public async Task<IActionResult> GetPreviousTermsClassSubjectScoreEntriesAsync(string sessionClassid, string subjectId, string sessionTermId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetPreviousTermsClassSubjectScoreEntriesAsync(Guid.Parse(sessionClassid), Guid.Parse(subjectId), Guid.Parse(sessionTermId), filter);
            return Ok(response);
        }

        [HttpPost("update/previous-terms/exam-score")]
        public async Task<IActionResult> UpdatePreviousTermsExamScore([FromBody] UpdateScore request)
        {
            var response = await service.UpdateExamScore(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/previous-terms/assessment-score")]
        public async Task<IActionResult> UpdatePreviousTermsAssessmentScore([FromBody] UpdateScore request)
        {
            var response = await service.UpdateAssessmentScore(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/previous-terms/preview-class/score-entries")]
        public async Task<IActionResult> PreviewPreviousTermsClassScoreEntry(string sessionClassid, string subjectId, string sessionTermId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.PreviewPreviousTermsClassScoreEntry(Guid.Parse(sessionClassid), Guid.Parse(subjectId), Guid.Parse(sessionTermId), filter);
            return Ok(response);
        }

        [HttpGet("get/student-result")]
        public async Task<IActionResult> GetStudentResultAsync(string sessionClassid, string termId, string studentContactId)
        {
            var response = await service.GetStudentResultForPreviewAsync(Guid.Parse(sessionClassid), Guid.Parse(termId), Guid.Parse(studentContactId));
            return Ok(response);
        }

        [HttpPost("print/result")]
        public async Task<IActionResult> PrintResultAsync([FromBody] PrintResultRequest request)
        {
            var response = await service.PrintResultAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("get/students/for-batch-printing")]
        public async Task<IActionResult> GetStudentsForBachPrinting([FromBody] BatchPrintResultRequest1 request)
        {
            var response = await service.GetStudentsForBachPrinting(request.SessionClassid, request.TermId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("batch-print/students-results")]
        public async Task<IActionResult> BatchPrintResult([FromBody] BatchPrintResultRequest2 request)
        {
            var response = await service.PrintBatchResultResultAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/publish-list")]
        public async Task<IActionResult> GetPublishedList()
        {
            var response = await service.GetPublishedList();
            return Ok(response);
        }
    }
} 