﻿using API.Controllers.BaseControllers;
using BLL.ClassServices;
using BLL.MiddleWares;
using Contracts.Class;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("class/api/v1/")]
    public class ClassController : BaseController
    { 
        private readonly IClassService service;
        private readonly IClassLookupService lookupService;
        public ClassController(IClassService service, IClassLookupService lookupService)
        {
            this.lookupService = lookupService;
            this.service = service;
        }


        #region CLASS LOOKUPS

        [HttpPost("create/class-lookup")]
        public async Task<IActionResult> CreateClassLookupAsync([FromBody] ApplicationLookupCommand request)
        {
            var response = await lookupService.CreateClassLookupAsync(request.Name, Guid.Parse(request.GradeLevelId));
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/class-lookup")]
        public async Task<IActionResult> UpdateClassLookupAsync([FromBody] ApplicationLookupCommand request)
        {
            var response = await lookupService.UpdateClassLookupAsync(request.Name, request.LookupId, request.IsActive, Guid.Parse(request.GradeLevelId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/class-lookup")]
        public async Task<IActionResult> GetAllClassLookupsAsync()
        {
            var response = await lookupService.GetAllClassLookupsAsync();
            return Ok(response);
        }

        [HttpGet("get-all/active-classes")]
        public async Task<IActionResult> GetActiveClassesAsync()
        {
            var response = await lookupService.GetAllActiveClassLookupsAsync();
            return Ok(response);
        }

        [HttpPost("delete/class-lookup")]
        public async Task<IActionResult> DeleteClassLookupAsync([FromBody] MultipleDelete reguest)
        {
            var response = await lookupService.DeleteClassLookupAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion

        #region CLASS

        [HttpPost("create/session-class")]
        public async Task<IActionResult> CreateClassAsync([FromBody] SessionClassCommand request)
        {
            var response = await service.CreateSessionClassAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/session-class")]
        public async Task<IActionResult> UpdateClassAsync([FromBody] SessionClassCommand request)
        {
            var response = await service.UpdateSessionClassAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-all/session-classes{sessionId}")]
        public async Task<IActionResult> GetClassesAsync(Guid sessionId)
        {
            var response = await service.GetSessionClassesAsync(sessionId);
            return Ok(response);
        }

       
        [HttpGet("search/classes/by-session")]
        public async Task<IActionResult> GetClassesBySessionAsync([FromBody] SessionQuery query)
        {
            var response = await service.GetSessionClassesBySessionAsync(query.StartDate, query.EndDate);
            return Ok(response);
        }

        [HttpGet("get-single/session-classes/{sessionClassId}")]
        public async Task<IActionResult> GetSingleSessionClassAsync(string sessionClassId)
        {
            var response = await service.GetSingleSessionClassesAsync(Guid.Parse(sessionClassId));
            return Ok(response);
        }

        [HttpGet("get-students/{sessionClassId}")]
        public async Task<IActionResult> GetSessionClassStudentsAsync(string sessionClassId)
        {
            var response = await service.GetClassStudentsClassesAsync(Guid.Parse(sessionClassId));
            return Ok(response);
        }

        [HttpPost("delete-session-class")]
        public async Task<IActionResult> DeleteSessionClassAsync([FromBody] SingleDelete reguest)
        {

            var response = await service.DeleteSessionClassesAsync(Guid.Parse(reguest.Item));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion


    }
} 