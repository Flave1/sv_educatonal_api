using BLL;
using BLL.EmailServices;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Email;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SMP.API.Hubs;
using SMP.BLL.Hubs;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.NotificationModels;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NotififcationServices
{
    public class NotificationService:  INotificationService
    {
        private readonly DataContext context;
        private readonly IEmailService emailService;
        private readonly IPaginationService paginationService;
        private readonly IHttpContextAccessor accessor;
        protected readonly IHubContext<NotificationHub> hub;
        public NotificationService(DataContext context, IEmailService emailService, IPaginationService paginationService, IHttpContextAccessor accessor, IHubContext<NotificationHub> hub)
        {
            this.context = context;
            this.emailService = emailService;
            this.paginationService = paginationService;
            this.accessor = accessor;
            this.hub = hub;
        }


        public async Task CreateNotitficationAsync(NotificationDTO request, bool byEmail, bool bySms)
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
                    Type = request.Type
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

        public async Task<APIResponse<PagedResponse<List<GetNotificationDTO>>>> GetNotitficationAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetNotificationDTO>>>();
            try
            {
                var query =  context.Notification.Where(x => !x.Deleted);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetNotificationDTO(f)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateNotification(Guid NotififcationId)
        {
            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var nt = await context.Notification.FindAsync(NotififcationId);
            if(nt != null)
            {
                var splitedIds = !string.IsNullOrEmpty(nt.ReadBy) ? nt.ReadBy.Split(',').ToList() : new List<string>();
                if (!splitedIds.Any(d => d == userId))
                {
                    splitedIds.Add(userId);
                    nt.ReadBy = string.Join(',', splitedIds);
                    await context.SaveChangesAsync();
                }
            }
            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());
        }

    }
}
