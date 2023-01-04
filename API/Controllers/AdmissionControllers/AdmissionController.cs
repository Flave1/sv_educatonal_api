using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AdmissionServices;
using System.Threading.Tasks;

namespace SMP.API.Controllers.AdmissionControllers
{
    [PortalAuthorize]
    [Route("smp/api/v1/admission")]
    public class AdmissionController: Controller
    {
        private readonly IAdmissionService service;
        public AdmissionController(IAdmissionService service)
        {
            this.service = service;
        }
        [HttpGet("get-all-admission")]
        public async Task<IActionResult> GetAllAdmission(PaginationFilter filter)
        {
            var response = await service.GetAllAdmission(filter);
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
    }
}
