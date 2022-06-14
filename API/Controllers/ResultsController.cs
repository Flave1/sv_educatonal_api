using API.Controllers.BaseControllers;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ResultServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("api/v1/result/")]
    public class ResultsController : BaseController
    {
        private readonly IResultsService service;
        public ResultsController(IResultsService service)
        {
            this.service = service;
        }

        [HttpGet("get/staff-classes")]
        public async Task<IActionResult> GetCurrentStaffClassesAsync()
        {
            var response = await service.GetCurrentStaffClassesAsync();
            return Ok(response);
        }

        [HttpGet("get/staff-class-subjects/{sessionClassid}")]
        public async Task<IActionResult> GetCurrentStaffClassSubjectsAsync(string sessionClassid)
        {
            var response = await service.GetCurrentStaffClassSubjectsAsync(Guid.Parse(sessionClassid));
            return Ok(response);
        }

        [HttpGet("get/class-score-entries/{sessionClassid}")]
        public async Task<IActionResult> GetClassSubjectScoreEntriesAsync(string sessionClassid)
        {
            var response = await service.GetClassEntryAsync(Guid.Parse(sessionClassid));
            return Ok(response);
        }
    }
} 