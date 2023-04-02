using BLL.MiddleWares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PortalService;
using SMP.Contracts.PortalSettings;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("portalsetting/api/v1/")]
    public class PortalSettingController : Controller
    {
        private readonly IPortalSettingService service;  
        public PortalSettingController(IPortalSettingService service)
        {
            this.service = service;
        }

        #region portalsetting


        [HttpPost("create-update/school-setting")]
        public async Task<IActionResult> CreateUpdateScoolSettingAsync([FromForm] PostSchoolSetting request)
        {

            var response = await service.CreateUpdateSchollSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create-update/result-setting")]
        public async Task<IActionResult> CreateUpdateResultSettingAsync([FromForm] PostResultSetting request)
        {

            var response = await service.CreateUpdateResultSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/result-setting-template")]
        public async Task<IActionResult> UpdateResultSettingTemplateAsync([FromBody] UpdateResultSetting request)
        { 
            var response = await service.UpdateResultSettingTemplateAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("create-update-notification-setting")]
        public async Task<IActionResult> CreateNotificationSettingAsync([FromBody] PostNotificationSetting request)
        {

            var response = await service.CreateUpdateNotificationSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/school-settings")]
        public async Task<IActionResult> GetSchoolSettings()
        {
            var response = await service.GetSchollSettingsAsync();
            return Ok(response);
        }
         
        [HttpGet("get/result-settings")]
        public async Task<IActionResult> GetResultSettings()
        {
            var response = await service.GetResultSettingsAsync();
            return Ok(response);
        }
         
        [HttpGet("get/notification-settings")]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var response = await service.GetNotificationSettingsAsync();
            return Ok(response);
        }

        [HttpPost("update-applayout-setting")]
        public async Task<IActionResult> UpdateAppLayoutSettingAsync([FromBody] AppLayoutSettings request)
        {

            var response = await service.UpdateAppLayoutSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("get/applayout-setting")]
        public async Task<IActionResult> GetAppLayoutSettingsAsync(string url)
        {
            var response = await service.GetAppLayoutSettingsAsync(url);
            return Ok(response);
        }

        [HttpPost("create-update/reg-no-setting")]
        public async Task<IActionResult> CreatUpdateRegNoSettingAsync([FromBody]CreateRegNoSetting request)
        {
            var response = await service.CreateUpdateRegNoSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/reg-no-setting")]
        public async Task<IActionResult> GetRegNoSetting()
        {
            var response = await service.GetRegNoSettingsAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion

    }
}
