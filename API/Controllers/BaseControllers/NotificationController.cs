using BLL;
using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.NotififcationServices;
using SMP.Contracts.NotificationModels;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("notification/api/v1/")]
    public class NotificationController : Controller
    {
        private readonly INotificationService service;
        public NotificationController(INotificationService notificationService)
        {
            this.service = notificationService;
        }

        [HttpGet("get-notifications")]
        public async Task<IActionResult> Getnotitfications(PaginationFilter filter)
        {
            var response = await service.GetNotitficationAsync(filter);
            return Ok(response);
        }


        [HttpGet("get-single-notifications")]
        public async Task<IActionResult> GetSingleNotitficationAsync(Guid notififcationId)
        {
            var response = await service.GetSingleNotitficationAsync(notififcationId);
            return Ok(response);
        }

        [HttpPost("update/notifications")]
        public async Task<IActionResult> UpdateProfileByStudentAsync([FromForm] UpdateNotification request)
        {
            await service.UpdateNotification(request.NotificationId);
            var rs = new APIResponse<bool>();
            rs.Message.FriendlyMessage = "success";
            return Ok(rs);
        }
    }

}