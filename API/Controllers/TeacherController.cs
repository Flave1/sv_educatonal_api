using BLL.AuthenticationServices;
using BLL.Constants;
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
        public async Task<IActionResult> UpdateTeacherAsync([FromBody] UserCommand request)
        {
            var response = await service.UpdateTeacherAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/teachers")]
        public async Task<IActionResult> GetAllTeachersAsync()
        {
            var response = await service.GetAllTeachersAsync();
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
        #endregion



    }
}
