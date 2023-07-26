using Microsoft.AspNetCore.Identity;

namespace DAL.Authentication
{
    public class AppUser : IdentityUser
    {
        public string UserTypes { get; set; } //could possibly be multiple User types
        public string FwsUserId { get; set; }
        public bool Active { get; set; }
        public string SocketId { get; set; }
    }
}
