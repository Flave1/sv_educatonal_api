using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SMP.API.Hubs
{
    public class NotificationHub: Hub
    {
        private readonly string _botUser;
        private readonly IHttpContextAccessor accessor;
        public NotificationHub(){}
        public NotificationHub(IHttpContextAccessor accessor)
        {
            _botUser = "Notification Bot";
            this.accessor = accessor;
        }
        public async Task JoinNotificationRoom(UserConnection conn)
        {
            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId");
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);
            await Clients.Groups(conn.Room).SendAsync("NotificationArea", _botUser, "successfully connected to notification room");
        }

        public async Task PushNotification(string message, string room, string user)
        {
            await Clients.Groups(room).SendAsync("NotificationArea", user, message);
        }
    }

}
