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
         
        [HttpPost("create/announcement")]
        public async Task<IActionResult> CreateAnnouncementsAsync([FromBody] CreateAnnouncement request)
        {
            var response = await service.CreateAnnouncementsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/announcement")]
        public async Task<IActionResult> UpdateAnnouncementsAsync([FromBody] UpdateAnnouncement request)
        {
            var response = await service.UpdateAnnouncementsAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("update/seen-announcement")]
        public async Task<IActionResult> UpdateSeenAnnouncementAsync([FromBody] UpdatSeenAnnouncement request)
        {
            var response = await service.UpdateSeenAnnouncementAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/announcements")]
        public async Task<IActionResult> GetAnnouncementsAsync()
        {
            var response = await service.GetAnnouncementsAsync();
            return Ok(response);
        }


    }
}
