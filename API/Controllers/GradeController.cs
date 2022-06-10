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
using SMP.BLL.Services.GradeServices;
using SMP.Contracts.Enrollment;
using SMP.Contracts.GradeModels;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("grade/api/v1/")]
    public class GradeController : Controller
    {
        private readonly IGradeService service;
        public GradeController(IGradeService service)
        {
            this.service = service;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGradeAsync([FromBody] EditGradeGroupModel request)
        {
            var response = await service.UpdateGradeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGradeAsync([FromBody] AddGradeGroupModel request)
        {
            var response = await service.CreateGradeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest();
        }

        [HttpGet("get/get-settings")]
        public async Task<IActionResult> getGradeAsync()
        {
            var response = await service.GetGradeSettingAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest();
        }

        [HttpGet("get/get-classes")]
        public async Task<IActionResult> GetClassesAsync()
        {
            var response = await service.GetClassesAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest();
        }

    }
}
