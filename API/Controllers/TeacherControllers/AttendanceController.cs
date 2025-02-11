﻿using BLL.MiddleWares;
using Contracts.Common; 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SMP.BLL.Services.AttendanceServices;
using Contracts.AttendanceContract;
using BLL.Filter;

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
        public async Task<IActionResult> CreateClassRegisterAsync([FromBody] CreateClassRegister request)
        {
            var response = await service.CreateClassRegisterAsync(request.SessionClassId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/student-attendance")]
        public async Task<IActionResult> UpdateStudentAttendanceRecord([FromBody] PostStudentAttendance request)
        {
            var response = await service.UpdateStudentAttendanceRecord(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/student-attendance-mobile")]
        public async Task<IActionResult> UpdateStudentAttendanceRecordMobile([FromBody] PostStudentAttendance2 request)
        {
            var response = await service.UpdateStudentAttendanceRecordFromMobile(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("continue-attendance")]
        public async Task<IActionResult> ContinueAttendanceAsync(Guid ClassRegisterId)
        {
            var response = await service.ContinueAttendanceAsync(ClassRegisterId);
            return Ok(response);
        }
        [HttpGet("get/present-students")]
        public async Task<IActionResult> GetAllStudentPresent(Guid classRegisterId)
        {
            var response = await service.GetAllStudentPresentAsync(classRegisterId);
            return Ok(response);
        }
        [HttpGet("get/absent-students")]
        public async Task<IActionResult> GetAllStudentAbsent(Guid classRegisterId)
        {
            var response = await service.GetAllAbsentStudents(classRegisterId);
            return Ok(response);
        }

        [HttpGet("get/all/class-register/{sessionClassId}")]
        public async Task<IActionResult> GetAllAttendanceRegisterAsync(string sessionClassId, string termId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetAllAttendanceRegisterAsync(sessionClassId, termId, filter);
            return Ok(response);
        }

        [HttpGet("get/all/class-register/activeterm/{sessionClassId}")]
        public async Task<IActionResult> GetAllAttendanceRegisterAsync(string sessionClassId, int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetAllAttendanceRegisterAsync(sessionClassId, filter);
            return Ok(response);
        }

        [HttpPost("delete/class-register")]
        public async Task<IActionResult> DeleteClassRegisterAsync(SingleDelete delete)
        {
            var response = await service.DeleteClassRegisterAsync(delete);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/class-register")]
        public async Task<IActionResult> UpdateClassRegisterLabelAsync(UpdateClassRegister classRegister)
        {
            var response = await service.UpdateClassRegisterLabel(classRegister);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
         
        #endregion
         
    }
}
