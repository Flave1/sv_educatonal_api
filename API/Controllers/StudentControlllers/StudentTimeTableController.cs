using BLL.MiddleWares; 
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.NoteServices;
using SMP.BLL.Services.TimetableServices;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("smp/studenttimetable/api/v1")]
    public class StudentTimeTableController : Controller
    {
        private readonly ITimeTableService service;
        public StudentTimeTableController(ITimeTableService service)
        {
            this.service = service;
        }


        [HttpGet("get/by-student")]
        public async Task<IActionResult> GetClassTimeTableByStudentAsync()
        {
            var response = await service.GetClassTimeTableByStudentAsync();
            return Ok(response);
        }
    }
}
