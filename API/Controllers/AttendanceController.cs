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


        [HttpPost("create-register")]
        public async Task<IActionResult> CreateRegisterAsync([FromBody] Guid SessionClassId)
        {

            var response = await service.CreateClassRegisterAsync(SessionClassId);
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
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] Guid ClassRegisterId)
        {
            
            var response = await service.ContinueAttendanceAsync(ClassRegisterId);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("present_student")]
        public async Task<IActionResult> PresentStudentAttendanceAsync([FromBody] Guid classRegisterId)
        {
            
            var response = await service.PresentStudentAsync(classRegisterId);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("absent_student")]
        public async Task<IActionResult> AbsentStudentAttendanceAsync([FromBody] Guid classRegisterId)
        {
            
            var response = await service.AbsentStudentAsync(classRegisterId);
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

        [HttpGet("delete-class-register")]
        public async Task<IActionResult> DeleteClassRegisterAsync(DeleteClassRegisterContract delete)
        {
            var response = await service.DeleteClassRegisterAsync(delete);
            return Ok(response);
        }

        [HttpGet("update-class-register")]
        public async Task<IActionResult> UpdateClassRegisterLabelAsync(UpdateClassRegisterContract ClassRegister)
        {
            var response = await service.UpdateClassRegisterLabel(ClassRegister);
            return Ok(response);
        }
         
        #endregion
         
    }
}
