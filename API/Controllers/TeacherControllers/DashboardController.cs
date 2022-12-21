using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.DashboardServices;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("dashboard/api/v1")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService service;
        public DashboardController(IDashboardService sertvice)
        {
            this.service = sertvice;
        }
         
        [HttpGet("get/dashboard-count")]
        public IActionResult GetDashboardCountAsync()
        {
            var response = service.GetDashboardCountAsync();
            return Ok(response);
        }

        [HttpGet("get-student/dashboard-count")]
        public IActionResult GetStudentDashboardCountAsync()
        {
            var response = service.GetStudentDashboardCountAsync();
            return Ok(response);
        }

        [HttpGet("get-mobile/dashboard-count")]
        public IActionResult GetTeacherMobileDashboardCountAsync()
        {
            var response = service.GetTeacherMobileDashboardCountAsync();
            return Ok(response);
        }
    }
}
