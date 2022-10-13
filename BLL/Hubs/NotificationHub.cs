using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SMP.BLL.Hubs;
using System.Threading.Tasks;

namespace SMP.API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly string _botUser;
        public NotificationHub()
        {
            _botUser = "Bot";
        }
        public async Task JoinNotificationRoom(NotificationArea conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);
            await Clients.Groups(NotificationRooms.PushedNotification).SendAsync("NotificationArea", conn.UserId, $"{conn.UserId} is joined");
        }

    }

}
