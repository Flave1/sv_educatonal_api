using BLL.MiddleWares;
using Contracts.Common;
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
        [HttpPost("update/class-timetable-time-activity")]
        public async Task<IActionResult> CreateClassTimeTableTimeActivity([FromBody] UpdateClassTimeTableTimeActivity request)
        {
            var response = await service.UpdateClassTimeTableTimeActivityAsync(request);
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

        [HttpPost("delete/class-timetable-time")]
        public async Task<IActionResult> DeleteClassTimeTableTimeAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteClassTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete/class-timetable-day")]
        public async Task<IActionResult> DeleteClassTimeTableDayAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteClassTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("delete/class-timetable-activity")]
        public async Task<IActionResult> UpdateClassTimeTableTimeActivityAsync([FromBody] UpdateClassTimeTableTimeActivity request)
        {
            var response = await service.UpdateClassTimeTableTimeActivityAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/class-timetable-time")]
        public async Task<IActionResult> UpdateClassTimeTableTimeAsync([FromBody] UpdateClassTimeTableTime request)
        {
            var response = await service.UpdateClassTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/class-timetable-day")]
        public async Task<IActionResult> UpdateClassTimeTableDayAsync([FromBody] UpdateClassTimeTableDay request)
        {
            var response = await service.UpdateClassTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
