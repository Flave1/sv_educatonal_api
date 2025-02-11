﻿using BLL.Filter;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AssessmentServices;
using SMP.Contracts.Assessment;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("classassessment/api/v1")]
    public class ClassAssessmentController : Controller
    {
        private readonly IClassAssessmentService service;
        public ClassAssessmentController(IClassAssessmentService service)
        {
            this.service = service;
        }

        [HttpGet("get-all/class-assessments")]
        public async Task<IActionResult> GetStudentClassAssessmentsAsync(string sessionClassId, string sessionClassSubjectId, int pageNumber)
        {
            PaginationFilter filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetAssessmentByTeacherAsync(sessionClassId, sessionClassSubjectId, filter);
            return Ok(response);
        }

        [HttpPost("ceate/class-assessment")]
        public async Task<IActionResult> CreateClassAssessmentsAsync([FromBody] CreateClassAssessment request)
        {
            var response = await service.CreateClassAssessmentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-students/class-assessment")]
        public async Task<IActionResult> GetStudentClassAssessmentsAsync(string classAssessmentId)
        {
            var response = await service.GetClassStudentByAssessmentAsync(Guid.Parse(classAssessmentId));
            return Ok(response);
        }

        [HttpPost("update-student/class-assessment")]
        public async Task<IActionResult> UpdateStudentAssessmentScoreAsync([FromBody] UpdateStudentAssessmentScore request)
        {
            var response = await service.UpdateStudentAssessmentScoreAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/class-assessment/score")]
        public async Task<IActionResult> UpdateClassAssessmentScoreAsync([FromBody] UpdatClassAssessmentScore request)
        {
            var response = await service.UpdateClassAssessmentScoreAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-single/class-assessments")]
        public async Task<IActionResult> GetSingleAssessmentAsync(string classAssessmentId)
        {
            var response = await service.GetSingleAssessmentAsync(Guid.Parse(classAssessmentId));
            return Ok(response);
        }


        [HttpPost("delete/class-assessment")]
        public async Task<IActionResult> DdeleteClassAssessmentsAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteClassAssessmentAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
