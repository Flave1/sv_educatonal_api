using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.EnrollmentServices;
using SMP.Contracts.Enrollment;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("errollment/api/v1/")]
    public class EnrollmentController : Controller
    { 
        private readonly IEnrollmentService service;
        public EnrollmentController(IEnrollmentService service)
        {
            this.service = service;
        }

        #region ENROLLMENT

        [HttpPost("enroll/students")]
        public async Task<IActionResult> EnrrollStudentAsync([FromBody] Enroll request)
        {
            var response = await service.EnrollStudentsAsyncAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("unenroll/students")]
        public async Task<IActionResult> UnenrrollStudentAsync([FromBody] UnEnroll request)
        {
            var response = await service.UnenrollStudentsAsyncAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/enrolled")]
        public async Task<IActionResult> GetAllEnrrolledStudentsAsync()
        {
            var response = await service.GetAllEnrrolledStudentsAsync();
            return Ok(response);
        }


        [HttpGet("getall/unenrolled")]
        public async Task<IActionResult> GetAllUnenrrolledStudentsAsync()
        {
            var response = await service.GetAllUnenrrolledStudentsAsync();
            return Ok(response);
        }

        #endregion

    }
}
