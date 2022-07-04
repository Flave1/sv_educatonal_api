using BLL.MiddleWares;
using Contracts.Common; 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SMP.BLL.Services.AttendanceServices;
using Contracts.AttendanceContract;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("attendance/api/v1/")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService service;  
        public AttendanceController(IAttendanceService service)
        {
            this.service = service; 
        }

        #region Attendance


        [HttpPost("create")]
        public async Task<IActionResult> CreateAttendanceAsync([FromBody] PostStudentAttendance request)
        {

            var response = await service.UpdateStudentAttendanceRecord(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAttendanceAsync([FromBody] PostStudentAttendance request)
        {
            
            var response = await service.UpdateStudentAttendanceRecord(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] PostStudentAttendance request)
        {
            
            var response = await service.ContinueAttendanceAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("present_student")]
        public async Task<IActionResult> PresentStudentAttendanceAsync([FromBody] PostStudentAttendance request)
        {
            
            var response = await service.PresentStudentAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("absent_student")]
        public async Task<IActionResult> AbsentStudentAttendanceAsync([FromBody] PostStudentAttendance request)
        {
            
            var response = await service.AbsentStudentAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAttendancesAsync()
        {
            var response = await service.GetAllAttendanceRegisterAsync();
            return Ok(response);
        }
         
        #endregion
         
    }
}
