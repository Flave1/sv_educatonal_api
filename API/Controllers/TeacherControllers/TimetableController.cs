using BLL.MiddleWares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.TimetableServices;
using SMP.Contracts.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.API.Controllers.TeacherControllers
{
    [PortalAuthorize]
    [AllowAnonymous]
    [Route("api/v1/smp/timetable")]
    public class TimetableController : Controller
    {
        private readonly ITimeTableService service;
        public TimetableController(ITimeTableService service)
        {
            this.service = service;
        }

        [HttpGet("get/active-classes")]
        public async Task<IActionResult> GetAllActiveClassesAsync()
        {
            var response = await service.GetAllActiveClassesAsync();
            return Ok(response);
        }

        [HttpGet("get/class-time-table/{classId}")]
        public async Task<IActionResult> GetClassTimeTableAsync(string classId)
        {
            var response = await service.GetClassTimeTableAsync(Guid.Parse(classId));
            return Ok(response);
        }

        [HttpPost("create/class-timetable-day")]
        public async Task<IActionResult> CreateClassTimeTableDay([FromBody] CreateClassTimeTableDay request)
        {
            var response = await service.CreateClassTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
     
        [HttpPost("create/class-timetable-time")]
        public async Task<IActionResult> CreateClassTimeTable([FromBody] CreateClassTimeTableTime request)
        {
            var response = await service.CreateClassTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("create/class-timetable-time-activity")]
        public async Task<IActionResult> CreateClassTimeTableTimeActivity([FromBody] CreateClassTimeTableTimeActivity request)
        {
            var response = await service.CreateClassTimeTableTimeActivityAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/class-time-table-by-day/{day}")]
        public async Task<IActionResult> GetClassTimeTableByDayAsync(string day)
        {
            var response = await service.GetClassTimeActivityByDayAsync(day);
            return Ok(response);
        }
    }
}
