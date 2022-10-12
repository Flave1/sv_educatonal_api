using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SMP.API.Hubs
{
    public class NotificationHub: Hub
    {
        private readonly string _botUser;
        private readonly IHttpContextAccessor accessor;
        public NotificationHub(IHttpContextAccessor accessor)
        {
            _botUser = "Notification Bot";
            this.accessor = accessor;
        }
        public async Task JoinNotificationRoom(NotificationArea conn)
        {
            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId");
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);
            await Clients.Groups(conn.Room).SendAsync("NotificationArea", _botUser, "successfully connected to notification room");
        }

        public async Task PushNotification(SendNotification msg)
        {
            await Clients.Groups(msg.Room).SendAsync("NotificationArea", msg.User, msg.Message);
        }
    }

}
