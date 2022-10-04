using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SMP.API.Hubs
{
    public class UserConnection
    {
        public string UserId { get; set; }
        public string User { get; set; }
        public string Room { get; set; }
    }

    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
        }
    }
}
