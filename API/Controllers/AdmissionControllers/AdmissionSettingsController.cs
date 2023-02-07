using BLL.Filter;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AdmissionServices;
using SMP.Contracts.AdmissionSettings;
using System.Threading.Tasks;

namespace SMP.API.Controllers.AdmissionControllers
{
    [PortalAuthorize]
    [Route("smp/api/v1/admission-settings")]
    public class AdmissionSettingsController: Controller
    {
        private readonly IAdmissionSettingService service;

        public AdmissionSettingsController(IAdmissionSettingService service)
        {
            this.service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSettings([FromBody] CreateAdmissionSettings request)
        {
            var response = await service.CreateSettings(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-settings")]
        public async Task<IActionResult> GetAllSettings(PaginationFilter filter)
        {
            var response = await service.GetAllSettings(filter);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-single-settings")]
        public async Task<IActionResult> GetSettings(string admissionSettingsId)
        {
            var response = await service.GetSettingsById(admissionSettingsId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSettings([FromBody] SingleDelete request)
        {
            var response = await service.DeleteSettings(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateAdmissionSettings request)
        {
            var response = await service.UpdateSettings(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
