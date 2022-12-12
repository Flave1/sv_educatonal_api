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
        [HttpGet("get-settings")]
        public async Task<IActionResult> GetSettings()
        {
            var response = await service.GetSettings();
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
    }
}
