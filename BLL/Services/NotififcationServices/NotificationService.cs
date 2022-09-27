using Contracts.Annoucements;
using DAL;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;

namespace SMP.BLL.Services.NotififcationServices
{
    public class NotificationService: Hub, INotificationService
    {
        private readonly DataContext context;
        public NotificationService(DataContext context)
        {
            this.context = context;
        }

        public void PushAnnouncementNotitfication(CreateAnnouncement request)
        {
            try
            {
                //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Hub>();
                if (request.AssignedTo == "teacher")
                {
                    //hubContext.Clients.Group("PushNotification").GetData(data);
                    var rooms = new List<string>();
                    rooms.Add("PushedNotification");
                    Clients.Groups(rooms).SendAsync("NotificationArea", "", request.Content);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
