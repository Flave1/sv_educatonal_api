using BLL.MiddleWares;
using Contracts.Common; 
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PortalService;
using SMP.Contracts.PortalSettings;
using System;
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
        [HttpPost("create-update-notification-setting")]
        public async Task<IActionResult> CreateNotificationSettingAsync([FromBody] PostNotificationSetting request)
        {

            var response = await service.CreateUpdateNotificationSettingsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/school-settings")]
        public async Task<IActionResult> GetSchoolSettings(Guid schoolSettingId)
        {
            var response = await service.GetSchollSettingsAsync(schoolSettingId);
            return Ok(response);
        }
         
        [HttpGet("get/result-settings")]
        public async Task<IActionResult> GetResultSettings(Guid resultlSettingId)
        {
            var response = await service.GetResultSettingsAsync(resultlSettingId);
            return Ok(response);
        }
         
        [HttpGet("get/notification-settings")]
        public async Task<IActionResult> GetNotificationSettings(Guid notificationSettingId)
        {
            var response = await service.GetNotificationSettingsAsync(notificationSettingId);
            return Ok(response);
        }
          
        #endregion
         
    }
}
