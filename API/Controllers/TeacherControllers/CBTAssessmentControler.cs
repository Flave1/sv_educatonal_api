
using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.CBTAssessmentServices;
using SMP.Contracts.Assessment;
using System.Threading.Tasks;

namespace SMP.API.Controllers.TeacherControllers
{
    [PortalAuthorize]
    [Route("cbtassessment/api/v1")]
    public class CBTAssessmentControler : Controller
    {
        private readonly ICBTAssessmentService service;
        public CBTAssessmentControler(ICBTAssessmentService service)
        {
            this.service = service;
        }

        [HttpGet("get/assessments")]
        public async Task<IActionResult> GetCBTAssessmentAsync(PaginationFilter filter, string sessionClassId)
        {
            var response = await service.GetCBTAssessmentsAsync(sessionClassId, filter.PageNumber);
            return Ok(response);
        }

        [HttpPost("include/assessments")]
        public async Task<IActionResult> InCludeAsAssessment([FromBody] InCludeAssessmentRequest req)
        {
            var response = await service.IncludeCBTAssessmentToScoreEntryAsAssessment(req.sessionClassId, req.subjectId, req.studentRegNos, req.Include, req.examId);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
