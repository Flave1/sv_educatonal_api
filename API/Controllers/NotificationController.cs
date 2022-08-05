using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SMP.API.Hubs;
using SMP.Contracts.NotificationModels;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("notifications/api/v1")]
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> hub;
        public NotificationController(IHubContext<NotificationHub> hub)
        {
            this.hub = hub;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateNotificationsAsync([FromBody] SendNotification request)
        {
            await hub.Clients.All.SendAsync("sendToReact", request.Message);
        }

      

    }
}
