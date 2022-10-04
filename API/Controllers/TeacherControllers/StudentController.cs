using BLL;
using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.Helpers;
using BLL.MiddleWares;
using BLL.PaginationService.Services;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using DAL;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("student/api/v1/")]
    public class StudentController : Controller
    { 
        private readonly IStudentService service;
        private readonly IUriService uriService;
        private readonly DataContext context;

        public StudentController(IStudentService service, IUriService uriService, DataContext context)
        {
            this.service = service;
            this.uriService = uriService;
            this.context = context;
        }

        #region STUDENTS

        [HttpPost("create/student")]
        public async Task<IActionResult> CreateStudentAsync([FromForm] StudentContactCommand request)
        {
            var response = await service.CreateStudenAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/student")]
        public async Task<IActionResult> UpdateStudentAsync([FromForm] StudentContactCommand request)
        {
            var response = await service.UpdateStudenAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/by-student")]
        public async Task<IActionResult> UpdateProfileByStudentAsync([FromForm] UpdateProfileByStudentRequest request)
        {
            var response = await service.UpdateProfileByStudentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/students")]
        public async Task<IActionResult> GetAllStudentsAsync(PaginationFilter filter)
        {
            var route = Request.Path.Value;
            PaginationFilter validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var response = await service.GetAllStudensAsync(filter);
            var totalRecords = await context.StudentContact.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<APIResponse<List<GetStudentContacts>>>(response, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("get-single/{StudentAccountId}")]
        public async Task<IActionResult> GetSingleStudentsAsync(string StudentAccountId)
        {
            var response = await service.GetSingleStudentAsync(Guid.Parse(StudentAccountId));
            return Ok(response);
        }


        [HttpPost("delete/student")]
        public async Task<IActionResult> DeleteStudentAsync([FromBody] MultipleDelete reguest)
        {
            var response = await service.DeleteStudentAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("upload/students")]
        public async Task<IActionResult> UploadStudentsAsync([FromForm] UploadClass file)
        {
            var response = await service.UploadStudentsAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion

    }
}
