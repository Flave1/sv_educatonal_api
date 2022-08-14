﻿using BLL.MiddleWares;
using Contracts.Annoucements;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.AnnouncementsServices;
using SMP.BLL.Services.DashboardServices;
using SMP.BLL.Services.PinManagementService;
using SMP.Contracts.PinManagement;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetDashboardCountAsync()
        {
            var response = await service.GetDashboardCountAsync();
            return Ok(response);
        }
    }
}