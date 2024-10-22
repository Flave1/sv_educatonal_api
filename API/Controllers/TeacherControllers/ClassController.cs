﻿using API.Controllers.BaseControllers;
using BLL.ClassServices;
using BLL.MiddleWares;
using Contracts.Class;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.Contracts.ClassModels;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("class/api/v1/")]
    public class ClassController : Controller
    { 
        private readonly IClassService service;
        private readonly IClassGroupService classGoupService;
        private readonly IClassLookupService lookupService;
        public ClassController(IClassService service, IClassLookupService lookupService, IClassGroupService classGroupService)
        {
            this.lookupService = lookupService;
            this.service = service;
            this.classGoupService = classGroupService;
        }


        #region CLASS LOOKUPS

        [HttpPost("create/class-lookup")]
        public async Task<IActionResult> CreateClassLookupAsync([FromBody] ApplicationLookupCommand request)
        {
            var response = await lookupService.CreateClassLookupAsync(request.Name, request.IsActive, Guid.Parse(request.GradeLevelId));
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
        public async Task<IActionResult> CreateClassAsync([FromBody] SessionClassCommand2 request)
        {
            var response = await service.CreateSessionClass2Async(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/session-class")]
        public async Task<IActionResult> UpdateSessionClassAsync([FromBody] SessionClassCommand2 request)
        {
            var response = await service.UpdateSessionClass2Async(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("add-update/session-class-subjects")]
        public async Task<IActionResult> AddUpdateSessionClassSubjectsAsync([FromBody] ClassSubjectcommand request)
        {
            var response = await service.CreateSessionClassSubjectsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-all/session-classes{sessionId}")]
        public async Task<IActionResult> GetSessionClassesAsync(string sessionId)
        {
            var response = await service.GetSessionClassesAsync(sessionId);
            return Ok(response);
        }

        [HttpGet("get-all/session-classes1/{sessionId}")]
        public async Task<IActionResult> GetSessionClasses1Async(string sessionId)
        {
            var response = await service.GetSessionClasses1Async(sessionId);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("get-all/session-classes2")]
        public async Task<IActionResult> GetSessionClasses2Async()
        {
            var response = await service.GetSessionClasses2Async();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("get-all/session-classes-cbt")]
        public async Task<IActionResult> GetSessionClassesCbtAsync(string clientId)
        {
            var response = await service.GetSessionClassesCbtAsync(clientId);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpGet("get/session-class-by-reg-no-cbt")]
        public async Task<IActionResult> GetSessionClassesCbtByRegNoAsync(string regNo, string clientId)
        {
            var response = await service.GetSessionClassesCbtByRegNoAsync(regNo, clientId);
            return Ok(response);
        }

        [HttpGet("search/classes/by-session")]
        public async Task<IActionResult> GetClassesBySessionAsync([FromBody] SessionQuery query)
        {
            var response = await service.GetSessionClassesBySessionAsync(query.StartDate, query.EndDate);
            return Ok(response);
        }

        [HttpGet("get-single/session-classes/without-subs-students/{sessionClassId}")]
        public async Task<IActionResult> GetSingleSessionClassesWithoutSubjectsAndStudentsAsync(string sessionClassId)
        {
            var response = await service.GetSingleSessionClassesWithoutSubjectsAndStudentsAsync(Guid.Parse(sessionClassId));
            return Ok(response);
        }

        [HttpGet("session-classes/subjetcs/{sessionClassId}")]
        public async Task<IActionResult> GetSessionClassSubjects(string sessionClassId)
        {
            var response = await service.GetSessionClassSubjects(Guid.Parse(sessionClassId));
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

        [HttpGet("get-students/mobile/{sessionClassId}")]
        public async Task<IActionResult> GetSessionClassStudentsMobileAsync(string sessionClassId)
        {
            var response = await service.GetClassStudentsClassesMobileAsync(Guid.Parse(sessionClassId));
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

        #region  GROUP

        [HttpPost("create/class-group")]
        public async Task<IActionResult> CreateClassGroupAsync([FromBody] CreateClassGroup request)
        {
            var response = await classGoupService.CreateClassGroupAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/class-group")]
        public async Task<IActionResult> UpdateClassGroupAsync([FromBody] UpdateClassGroup request)
        {
            var response = await classGoupService.UpdateClassGroupAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/class-group")]
        public async Task<IActionResult> GetAllClassGroupsAsync(string sessionClassId, string sessionClassSubjectId)
        {
            var response = await classGoupService.GetAllClassGroupsAsync(Guid.Parse(sessionClassId), Guid.Parse(sessionClassSubjectId));
            return Ok(response);
        }

        [HttpGet("getall/class-subjects")]
        public async Task<IActionResult> GetSingleClassSubjectssAsync(string sessionClassId)
        {
            var response = await classGoupService.GetSessionClassSubjectsAsync(Guid.Parse(sessionClassId));
            return Ok(response);
        }
     
        [HttpGet("getall/single/class-group")]
        public async Task<IActionResult> GetSingleClassGroupsAsync(string groupId, string sessionClassId)
        {
            var response = await classGoupService.GetSingleClassGroupsAsync(Guid.Parse(groupId), Guid.Parse(sessionClassId));
            return Ok(response);
        }

        [HttpPost("delete/class-group")]
        public async Task<IActionResult> DeleteClassAsync([FromBody] MultipleDelete reguest)
        {
            var response = await classGoupService.DeleteClassGroupAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpGet("getall/class-subjects-cbt")]
        public async Task<IActionResult> GetSessionClassSubjectsCbtAsync(string sessionClassId, string clientId)
        {
            var response = await classGoupService.GetSessionClassSubjectsCbtAsync(Guid.Parse(sessionClassId), clientId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion

    }
} 