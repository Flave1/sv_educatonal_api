using BLL.MiddleWares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
namespace SMP.API.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly string _botUser;
        private readonly IHttpContextAccessor accessor;
        private readonly IUserIdProvider provider;
        public CommentsHub(IHttpContextAccessor accessor, IUserIdProvider provider)
        {
            _botUser = "Comment Bot";
            this.accessor = accessor;
            this.provider = provider;
        }
        public async Task JoinCommentRoom(UserConnection conn)
        {
            //var identity = provider.GetUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);
            await Clients.Groups(conn.Room).SendAsync("CommentArea", _botUser, $"{conn.User} is Viewing");
        }

        public async Task JoinNotificationRoom(UserConnection conn)
        {
            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId");
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);
            await Clients.Groups(conn.Room).SendAsync("NotificationArea", _botUser, "successfully connected to notification room");
        }
        public async Task AddComment(string message, string room, string user)
        {
            await Clients.Groups(room).SendAsync("CommentArea", user, message);
        }
    }

   
}
