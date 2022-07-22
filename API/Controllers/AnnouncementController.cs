using BLL.MiddleWares;
using Contracts.Annoucements;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.AnnouncementsServices;
using SMP.BLL.Services.PinManagementService;
using SMP.Contracts.PinManagement;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("announcements/api/v1")]
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementsService service;
        public AnnouncementController(IAnnouncementsService sertvice)
        {
            this.service = sertvice;
        }
         
        [HttpPost("make/announcement")]
        public async Task<IActionResult> MakeAnnouncement([FromBody] AnnouncementsContract request)
        {
            var response = await service.CreateAnnouncementsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update/announcement")]
        public async Task<IActionResult> UpdateAnnouncement([FromBody] AnnouncementsContract request)
        {
            var response = await service.UpdateAnnouncementsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/announcement")]
        public async Task<IActionResult> GetAnnouncementAsync()
        {
            var response = await service.GetAnnouncementsAsync();
            return Ok(response);
        }
    }
}
