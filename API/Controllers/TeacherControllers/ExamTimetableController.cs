using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.TimetableServices;
using SMP.Contracts.Timetable;
using System.Threading.Tasks;
using System;

namespace SMP.API.Controllers.TeacherControllers
{
    [PortalAuthorize]
    [Route("smp/api/v1/exam-timetable")]
    public class ExamTimetableController : Controller
    {
        private readonly IExamTimeTableService service;
        public ExamTimetableController(IExamTimeTableService service)
        {
            this.service = service;
        }

        [HttpGet("get/active-classes")]
        public async Task<IActionResult> GetAllActiveClassesAsync()
        {
            var response = await service.GetAllActiveClassesAsync();
            return Ok(response);
        }

        [HttpGet("get/exam-time-table/{classId}")]
        public async Task<IActionResult> GetExamTimeTableAsync(string classId)
        {
            var response = await service.GetExamTimeTableAsync(Guid.Parse(classId));
            return Ok(response);
        }

        [HttpPost("create/exam-timetable-day")]
        public async Task<IActionResult> CreateExamTimeTableDay([FromBody] CreateExamTimeTableDay request)
        {
            var response = await service.CreateExamTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create/exam-timetable-time")]
        public async Task<IActionResult> CreateExamTimeTable([FromBody] CreateExamTimeTableTime request)
        {
            var response = await service.CreateExamTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/exam-timetable-time-activity")]
        public async Task<IActionResult> CreateExamTimeTableTimeActivity([FromBody] UpdateExamTimeTableTimeActivity request)
        {
            var response = await service.UpdateExamTimeTableTimeActivityAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/exam-time-table-by-day/{day}")]
        public async Task<IActionResult> GetExamTimeTableByDayAsync(string day)
        {
            var response = await service.GetExamTimeActivityByDayAsync(day);
            return Ok(response);
        }

        [HttpPost("delete/exam-timetable-time")]
        public async Task<IActionResult> DeleteExamTimeTableTimeAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteExamTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete/exam-timetable-day")]
        public async Task<IActionResult> DeleteExamTimeTableDayAsync([FromBody] SingleDelete request)
        {
            var response = await service.DeleteExamTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("delete/exam-timetable-activity")]
        public async Task<IActionResult> UpdateExamTimeTableTimeActivityAsync([FromBody] UpdateExamTimeTableTimeActivity request)
        {
            var response = await service.UpdateExamTimeTableTimeActivityAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/exam-timetable-time")]
        public async Task<IActionResult> UpdateExamTimeTableTimeAsync([FromBody] UpdateExamTimeTableTime request)
        {
            var response = await service.UpdateExamTimeTableTimeAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/exam-timetable-day")]
        public async Task<IActionResult> UpdateExamTimeTableDayAsync([FromBody] UpdateExamTimeTableDay request)
        {
            var response = await service.UpdateExamTimeTableDayAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-all/exam-time-table")]
        public async Task<IActionResult> GetAllExamTimeTableAsync()
        {
            var response = await service.GetAllExamTimeTableAsync();
            return Ok(response);
        }
    }
}
