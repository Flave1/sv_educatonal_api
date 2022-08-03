using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("student/api/v1/")]
    public class StudentController : Controller
    { 
        private readonly IStudentService service;
        public StudentController(IStudentService service)
        {
            this.service = service;
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
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            var response = await service.GetAllStudensAsync();
            return Ok(response);
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

        #endregion

    }
}
