using BLL.MiddleWares;
using BLL.AttendanceServices;
using Contracts.Common; 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SMP.BLL.Services.AttendanceServices;
using Contracts.AttendanceContract;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("Attendance/api/v1/")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService AttendanceService;  
        public AttendanceController(IAttendanceService service)
        {
            this.AttendanceService = service; 
        }

        #region Attendance

        [HttpPost("create")]
        public async Task<IActionResult> CreateAttendanceAsync([FromBody] PostAttendance request)
        {
            
            var response = await AttendanceService.CreateAttendanceAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] PostAttendance request)
        {
            
            var response = await AttendanceService.UpdateAttendanceAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("present_student")]
        public async Task<IActionResult> PresentStudentAttendanceAsync([FromBody] PostAttendance request)
        {
            
            var response = await AttendanceService.PresentStudentAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("absent_student")]
        public async Task<IActionResult> AbsentStudentAttendanceAsync([FromBody] PostAttendance request)
        {
            
            var response = await AttendanceService.AbsentStudentAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAttendancesAsync()
        {
            var response = await AttendanceService.GetAllAttendanceRegisterAsync();
            return Ok(response);
        }
         
        #endregion
         
    }
}
