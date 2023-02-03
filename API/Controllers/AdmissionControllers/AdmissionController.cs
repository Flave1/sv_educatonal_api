using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AdmissionServices;
using SMP.Contracts.Admissions;
using System.Threading.Tasks;

namespace SMP.API.Controllers.AdmissionControllers
{
    [PortalAuthorize]
    [Route("smp/api/v1/admission")]
    public class AdmissionController: Controller
    {
        private readonly IAdmissionService service;
        private readonly ICandidateAdmissionService candidateAdmissionService;

        public AdmissionController(IAdmissionService service, ICandidateAdmissionService candidateAdmissionService)
        {
            this.service = service;
            this.candidateAdmissionService = candidateAdmissionService;
        }
        [HttpGet("get-all-admission")]
        public async Task<IActionResult> GetAllAdmission(PaginationFilter filter, string classId, string examStatus , string admissionSettingsId)
        {
            var response = await service.GetAllAdmission(filter, classId, examStatus, admissionSettingsId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-admission/{admissionId}")]
        public async Task<IActionResult> GetAdmission(string admissionId)
        {
            var response = await service.GetAdmission(admissionId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("admission/export-to-cbt")]
        public async Task<IActionResult> ExportCandidatesToCbt([FromBody]ExportCandidateToCbt request)
        {
            var response = await service.ExportCandidatesToCbt(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-admission-classes")]
        public async Task<IActionResult> GetAdmissionClasses()
        {
            var response = await candidateAdmissionService.GetAdmissionClasses();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("enroll-candidate")]
        public async Task<IActionResult> EnrollCandidate([FromBody]EnrollCandidate request)
        {
            var response = await service.EnrollCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("enroll-candidates")]
        public async Task<IActionResult> EnrollCandidates([FromBody]EnrollCandidates request)
        {
            var response = await service.EnrollMultipleCandidates(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("import-result")]
        public async Task<IActionResult> ImportResult(string classId)
        {
            var response = await service.ImportCbtResult(classId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
