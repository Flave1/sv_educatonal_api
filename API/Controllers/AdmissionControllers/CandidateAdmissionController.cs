﻿using BLL.Filter;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AdmissionServices;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using System.Threading.Tasks;

namespace SMP.API.Controllers.AdmissionControllers
{
    [PortalAuthorize]
    [Route("smp/api/v1/candidate-admission")]
    public class CandidateAdmissionController: Controller
    {
        private readonly ICandidateAdmissionService service;
        public CandidateAdmissionController(ICandidateAdmissionService service)
        {
            this.service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateAdmission request)
        {
            var response = await service.CreateAdmission(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] UpdateAdmission request)
        {
            var response = await service.UpdateAdmission(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-admission")]
        public async Task<IActionResult> GetAllAdmission(PaginationFilter filter, string admissionSettingsId)
        {
            var response = await service.GetAllAdmission(filter, admissionSettingsId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-single-admission/{admissionId}")]
        public async Task<IActionResult> GetAdmission(string admissionId)
        {
            var response = await service.GetAdmission(admissionId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete-admission")]
        public async Task<IActionResult> DeleteAdmission([FromBody] SingleDelete request)
        {
            var response = await service.DeleteAdmission(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-admission-classes")]
        public async Task<IActionResult> GetAdmissionClasses()
        {
            var response = await service.GetAdmissionClasses();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-settings")]
        public async Task<IActionResult> GetAdmissionSettings()
        {
            var response = await service.GetSettings();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmEmail request)
        {
            var response = await service.ConfirmEmail(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-school-settings")]
        public async Task<IActionResult> GetSchoolSettings()
        {
            var response = await service.GetSchollSettingsAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("register-parent")]
        public async Task<IActionResult> RegisterParent([FromBody]CreateAdmissionParent request)
        {
            var response = await service.RegisterParent(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("resend-email-to/registered-parent")]
        public async Task<IActionResult> ResendEmailTORegisteredParent([FromBody] ResendRegEmail request)
        {
            var response = await service.ResendEmailTORegisteredParent(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
