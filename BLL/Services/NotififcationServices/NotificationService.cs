using BLL.EmailServices;
using Contracts.Annoucements;
using Contracts.Email;
using DAL;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SMP.Contracts.NotificationModels;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NotififcationServices
{
    public class NotificationService: Hub, INotificationService
    {
        private readonly DataContext context;
        private readonly IEmailService emailService;
        public NotificationService(DataContext context, IEmailService emailService)
        {
            this.context = context;
            this.emailService = emailService;
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateNotitfication(NotificationDTO request, bool byEmail, bool bySms)
        {
            try
            {
                var item = new Notification
                {
                    NotificationSourceId = request.NotificationSourceId,
                    Subject = request.Subject,
                    Content = request.Content,
                    Senders = request.Senders,
                    Receivers = request.Receivers,
                    IsRead = false,
                    IsSent = false,
                    NotificationEmailLink = request.NotificationEmailLink,
                    NotificationPageLink = request.NotificationPageLink,
                    Svg = request.Svg,
                };
                context.Notification.Add(item);
                await context.SaveChangesAsync();
                if(byEmail)
                    SendNotificationByEmail(item);
                //if (bySms)
                    //SendNotificationByEmail(item);
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task SendNotificationByEmail(Notification item)
        {
            var mail = new EmailMessage();
            mail.Subject = item.Subject;
            mail.Content = item.Content;
            mail.SentBy = item.Senders.Split(',').ToList().FirstOrDefault();
            var senders = item.Senders.Split(',').ToList().Select(x => new EmailAddress { Address = x, Name = x }).ToList();
            var receivers = item.Receivers.Split(',').ToList().Select(x => new EmailAddress { Address = x, Name = x }).ToList();
            mail.FromAddresses.AddRange(senders);
            mail.ToAddresses.AddRange(receivers);
            emailService.Send(mail);
        }

    }
}
