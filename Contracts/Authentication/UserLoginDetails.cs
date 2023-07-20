using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Authentication
{
    public class UserLoginDetails
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string SocketId { get; set; }
    }
}
