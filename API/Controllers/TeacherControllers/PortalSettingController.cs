﻿using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PinManagementService;
using SMP.BLL.Services.PortalService;
using SMP.Contracts.PinManagement;
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
        private readonly IPinManagementService pinservice;
        public PortalSettingController(IPortalSettingService service, IPinManagementService pinservice)
        {
            this.service = service;
            this.pinservice = pinservice;
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
        
          
        #endregion
         
    }
}