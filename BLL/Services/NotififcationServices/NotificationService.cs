using BLL;
using BLL.Constants;
using BLL.EmailServices;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Annoucements;
using Contracts.Email;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using SMP.API.Hubs;
using SMP.BLL.Hubs;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.NotificationModels;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
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
        private readonly ILoggerService loggerService;
        private readonly IWebRequestService webRequestService;
        private readonly string smsClientId;
        private readonly FwsConfigSettings fwsOptions;

        public NotificationService(DataContext context, IEmailService emailService, IPaginationService paginationService, IHttpContextAccessor accessor, 
            IHubContext<NotificationHub> hub, ILoggerService loggerService, IOptions<FwsConfigSettings> options, IWebRequestService webRequestService)
        {
            this.context = context;
            this.emailService = emailService;
            this.paginationService = paginationService;
            this.accessor = accessor;
            this.hub = hub;
            this.loggerService = loggerService;
            this.webRequestService = webRequestService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            fwsOptions = options.Value;
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
                    ReceiversEmail = request.Receivers,
                    IsRead = false,
                    IsSent = false,
                    NotificationEmailLink = request.NotificationEmailLink,
                    NotificationPageLink = request.NotificationPageLink,
                    Svg = request.Svg,
                    Type = request.Type,
                    ToGroup = request.ToGroup
                };
                context.Notification.Add(item);
                await context.SaveChangesAsync();
                if(byEmail)
                    SendNotificationByEmail(item);
                //if (bySms)
                //SendNotificationByEmail(item);
                //PUsh

                await PushNotification(request);

            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
            var receivers = item.ReceiversEmail.Split(',').ToList().Select(x => new EmailAddress { Address = x, Name = x }).ToList();
            mail.FromAddresses.AddRange(senders);
            mail.ToAddresses.AddRange(receivers);
            emailService.Send(mail);
        }

        public async Task<APIResponse<PagedResponse<List<GetNotificationDTO>>>> GetNotitficationAsync(PaginationFilter filter)
        {

            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var res = new APIResponse<PagedResponse<List<GetNotificationDTO>>>();
            try
            {
                var query =  context.Notification.Where(x=>x.ClientId == smsClientId).OrderByDescending(x => x.CreatedOn).Where(x => !x.Deleted);

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.AdminRole(smsClientId)))
                {
                    if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                    {
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin || x.ToGroup == NotificationRooms.Teachers);
                    }
                    else
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Teachers);
                }

                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.StudentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Students);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.ParentRole(smsClientId)))
                {   
                    query = query.Where(x => x.ToGroup == NotificationRooms.Parents);
                }

                query = paginationService.GetPagedResult(query, filter);

                query = query.Where(x => x.Receivers == "all" || x.Receivers.Contains(userId));


                var totaltRecord = query.Count();
                var result = await query.Select(f => new GetNotificationDTO(f)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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


        public async Task<APIResponse<GetNotificationDTO>> GetSingleNotitficationAsync(Guid notificationId)
        {

            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var res = new APIResponse<GetNotificationDTO>();
            try
            {
               res.Result = await context.Notification
                    .Where(x => x.ClientId == smsClientId && x.NotificationId == notificationId).Select(d => new GetNotificationDTO(d)).FirstOrDefaultAsync();
                
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
        private async Task PushNotification(NotificationDTO request)
        {
            string socketIds = "";
            if(request.Receivers != "all")
            {
                socketIds = string.Join(",", context.Users.Where(x => request.Receivers.Contains(x.Email)).Select(x => x.SocketId).ToList());
            }

            var notificationRequest = new PushNotificationDto
            {
                AnnouncementId = request.NotificationSourceId.ToString(),
                Subject = request.Subject,
                Content = request.Content,
                NotificationSourceId = request.NotificationSourceId.ToString(),
                NotificationPageLink = request.NotificationPageLink,
                Type = request.Type,
                Group = request.ToGroup,
                DateCreated = DateTime.Now.ToString("dd MMM, yyyy HH:mm:ss"),
                ClientId = smsClientId,
                Assignees = socketIds != "" ? socketIds.Split(",").ToList().Select(x => new Assignees { Id = x }).ToList() : new List<Assignees>()
            };

            await webRequestService.PostAsync<AnnouncementResponse, PushNotificationDto>($"{fwsOptions.FwsBaseUrl}{NotificationRoutes.createAnnouncement}", notificationRequest);
        }

        public async Task<APIResponse<GetNotificationDTO>> GetMostRecentNotificationAsync()
        {

            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var res = new APIResponse<GetNotificationDTO>();
            try
            {
                var query = context.Notification.Where(x => x.ClientId == smsClientId).OrderByDescending(x => x.CreatedOn).Where(x => !x.Deleted);

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.AdminRole(smsClientId)))
                {
                    if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                    {
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin || x.ToGroup == NotificationRooms.Teachers);
                    }
                    else
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Teachers);
                }

                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.StudentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Students);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.ParentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Parents);
                }

                query = query.Where(x => x.Receivers == "all" || x.Receivers.Contains(userId));


                var result = await query.Select(f => new GetNotificationDTO(f)).FirstOrDefaultAsync();
                res.Result = result;

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        public async Task<APIResponse<PagedResponse<List<GetNotificationDTO>>>> GetUnreadNotificationAsync(PaginationFilter filter)
        {

            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var res = new APIResponse<PagedResponse<List<GetNotificationDTO>>>();
            try
            {
                var query = context.Notification.Where(x => x.ClientId == smsClientId).OrderByDescending(x => x.CreatedOn).Where(x => !x.Deleted);

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.AdminRole(smsClientId)))
                {
                    if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                    {
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin || x.ToGroup == NotificationRooms.Teachers);
                    }
                    else
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Teachers);
                }

                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.StudentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Students);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.ParentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Parents);
                }

                query = paginationService.GetPagedResult(query, filter);

                query = query.Where(x => x.Receivers == "all" || x.Receivers.Contains(userId));

                query = query.Where(x => string.IsNullOrEmpty(x.ReadBy) || !x.ReadBy.Contains(userId));

                var totaltRecord = query.Count();
                var result = await query.Select(f => new GetNotificationDTO(f)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        public async Task<APIResponse<int>> GetUnreadNotificationCountAsync()
        {

            var userId = accessor.HttpContext.User.FindFirst(x => x.Type == "userId").Value;
            var res = new APIResponse<int>();
            try
            {
                var query = context.Notification.Where(x => x.ClientId == smsClientId).OrderByDescending(x => x.CreatedOn).Where(x => !x.Deleted);

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.AdminRole(smsClientId)))
                {
                    if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                    {
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin || x.ToGroup == NotificationRooms.Teachers);
                    }
                    else
                        query = query.Where(x => x.ToGroup == NotificationRooms.Admin);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Teachers);
                }

                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.StudentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Students);
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.ParentRole(smsClientId)))
                {
                    query = query.Where(x => x.ToGroup == NotificationRooms.Parents);
                }

                query = query.Where(x => x.Receivers == "all" || x.Receivers.Contains(userId));

                query = query.Where(x => string.IsNullOrEmpty(x.ReadBy) || !x.ReadBy.Contains(userId));

                var count = query.Count();
                res.Result = count;

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }
    }
}
