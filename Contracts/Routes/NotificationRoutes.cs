using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Routes
{
    public class NotificationRoutes
    {
        public const string RouteName = "job/server/";
        public const string createUser = RouteName + "user/create";
        public const string createAnnouncement = RouteName + "announcement/create";
    }
}
