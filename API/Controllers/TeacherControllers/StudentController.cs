using BLL.Filter;
using BLL.MiddleWares;
using BLL.PaginationService.Services;
using BLL.StudentServices;
using Contracts.Common;
using Contracts.Options;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SMP.Contracts.Students;
using System;
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
            var response = await service.GetAllStudensAsync(filter);
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
        [HttpPost("upload/students")]
        public async Task<IActionResult> UploadStudentsAsync([FromForm] UploadClass file)
        {
            var response = await service.UploadStudentsAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpGet("get-student-contact-cbt")]
        public async Task<IActionResult> GetSingleStudentByRegNoCbt(string studentRegNo)
        {
            var response = await service.GetSingleStudentByRegNoCbtAsync(studentRegNo);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpGet("getall-student-contact-cbt")]
        public async Task<IActionResult> GetSingleStudentBySessionClassCbt(PaginationFilter filter, string classId)
        {
            var response = await service.GetStudentBySessionClassCbtAsync(filter, classId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion

    }
}
