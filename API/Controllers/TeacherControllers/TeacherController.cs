using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.TeacherServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("tercher/api/v1")]
    public class TeacherController : Controller
    {
        private readonly ITeacherService service;
        public TeacherController(ITeacherService sertvice)
        {
            this.service = sertvice;
        }


        #region TEACHERS


        [HttpPost("/create/teacher")]
        public async Task<IActionResult> CreateTeacherAsync([FromForm] UserCommand request)
        {
            var response = await service.CreateTeacherAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("/update/teacher")]
        public async Task<IActionResult> UpdateTeacherAsync([FromForm] UserCommand request)
        {
            var response = await service.UpdateTeacherAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/teachers")]
        public async Task<IActionResult> GetAllTeachersAsync(PaginationFilter filter)
        {
            var response = await service.GetAllTeachersAsync(filter);
            return Ok(response);
        }

        [HttpGet("get-single/{teacherAccountId}")]
        public async Task<IActionResult> GetTeacherAsync(string teacherAccountId)
        {
            var response = await service.GetSingleTeacherAsync(Guid.Parse(teacherAccountId));
            return Ok(response);
        }

        [HttpPost("delete/teacher")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] MultipleDelete reguest)
        {

            var response = await service.DeleteTeacherAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/active-teachers")]
        public async Task<IActionResult> GetAllActiveUserAsync()
        {
            var response = await service.GetAllActiveTeachersAsync();
            return Ok(response);
        }

        [HttpPost("/update/teacher-profile/by-teacher")]
        public async Task<IActionResult> UpdateTeacherProfileByTeacherAsync([FromForm] UpdateProfileByTeacher request)
        {
            var response = await service.UpdateTeacherProfileByTeacherAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-teacher/classes-subject")]
        public async Task<IActionResult> GetSingleTeacherClassesAndSubjectsAsync(string teacherAccountId)
        {
            var response = await service.GetSingleTeacherClassesAndSubjectsAsync(Guid.Parse(teacherAccountId));
            return Ok(response);
        }

        [AllowAnonymous]

        [HttpPost("create-school-admin-user")]
        public async Task<IActionResult> CreateSchoolAdminUser([FromBody] UserCommand request)
        {
            var response = await service.CreateAdminAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        #endregion



    }
}
