using BLL;
using BLL.Constants;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Annoucements;
using Contracts.Common;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SMP.API.Hubs;
using SMP.BLL.Constants;
using SMP.BLL.Hubs;
using SMP.BLL.Services.AnnouncementsServices;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.NotififcationServices;
using SMP.Contracts.NotificationModels;
using SMP.DAL.Models.Annoucement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AnnouncementServices
{
    public class AnnouncementService : IAnnouncementsService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly INotificationService notificationService;
        protected readonly IHubContext<NotificationHub> hub;

        public AnnouncementService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService, IHubContext<NotificationHub> hub, INotificationService notificationService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.hub = hub;
            this.notificationService = notificationService;
        }

        async Task<APIResponse<GetAnnouncements>> IAnnouncementsService.UpdateSeenAnnouncementAsync(UpdatSeenAnnouncement request)
        {
            var res = new APIResponse<GetAnnouncements>();
            var userId = accessor.HttpContext.User.FindFirst(d => d.Type == "userId").Value;
            var announcement = await context.Announcement.Include(d => d.Sender).FirstOrDefaultAsync(x=>x.AnnouncementsId == Guid.Parse(request.AnnouncementsId));
            if(announcement != null)
            {
                var splitedIds = !string.IsNullOrEmpty(announcement.SeenByIds) ? announcement.SeenByIds.Split(',').ToList() : new List<string>();
                if(!splitedIds.Any(d => d == userId))
                {
                    splitedIds.Add(userId);
                    announcement.SeenByIds = string.Join(',', splitedIds);
                    await context.SaveChangesAsync();
                    
                }
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = new GetAnnouncements(announcement, userId);
                res.IsSuccessful = true;
                
                return res;
            }
            else
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
        }

        async Task<APIResponse<PagedResponse<List<GetAnnouncements>>>> IAnnouncementsService.GetAnnouncementsAsync(PaginationFilter filter)
        { 
            var res = new APIResponse<PagedResponse<List<GetAnnouncements>>>();
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            
            if (!string.IsNullOrEmpty(userid))
            {
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
                {
                    var query = context.Announcement
                          .Include(d => d.Sender)
                        .OrderByDescending(d => d.CreatedOn)
                        .Where(d => d.Deleted == false);

                    var totaltRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetAnnouncements(x, userid)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;
                }

                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    var query =  context.Announcement
                         .Include(d => d.Sender)
                         .OrderByDescending(d => d.CreatedOn)
                         .Where(d => d.AssignedTo == "teacher" && d.Deleted == false);

                    var totaltRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetAnnouncements(x, userid)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;
                }
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.STUDENT))
                {
                    var query = context.Announcement
                          .Include(d => d.Sender)
                        .OrderByDescending(d => d.CreatedOn)
                        .Where(d => d.AssignedTo == "student" && d.Deleted == false);

                    var totaltRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetAnnouncements(x, userid)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);


                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;

                }
             
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        async Task<APIResponse<CreateAnnouncement>> IAnnouncementsService.CreateAnnouncementsAsync(CreateAnnouncement request)
        {
            var res = new APIResponse<CreateAnnouncement>();
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var newAnnouncement = new Announcements()
            {
                Header = request.Header,
                AnnouncementDate = DateTime.UtcNow,
                AssignedTo = request.AssignedTo,
                SentBy = userid,
                Content = request.Content
            };
            
            await context.Announcement.AddAsync(newAnnouncement);
            await context.SaveChangesAsync();

            await notificationService.CreateNotitficationAsync(new NotificationDTO
            {
                Content = newAnnouncement.Content,
                NotificationPageLink = $"dashboard/smp-notification/announcement-details?announcementsId={newAnnouncement.AnnouncementsId}",
                NotificationSourceId = newAnnouncement.AnnouncementsId.ToString(),
                Subject = newAnnouncement.Header,
                Receivers = "all",
                Type = "announcement",
                ToGroup = request.AssignedTo
            });
            await hub.Clients.Group(NotificationRooms.PushedNotification).SendAsync(Methods.NotificationArea, new DateTime());

            res.Message.FriendlyMessage = Messages.Created;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<UpdateAnnouncement>> IAnnouncementsService.UpdateAnnouncementsAsync(UpdateAnnouncement request)
        {
            var res = new APIResponse<UpdateAnnouncement>();
            var ann = await context.Announcement.FirstOrDefaultAsync(d => d.AnnouncementsId == request.AnnouncementsId);
            if(ann == null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            ann.Header = request.Header;
            ann.AssignedTo = request.AssignedTo;
            ann.Content = request.Content;
            ann.IsEdited = true;
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Updated;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<bool>> IAnnouncementsService.DeleteAnnouncementsAsync(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            var result = await context.Announcement.FirstOrDefaultAsync(d => d.AnnouncementsId == Guid.Parse(request.Item));
            if (result != null)
            {
                result.Deleted = true;
                await context.SaveChangesAsync();
            }
            else
            { 
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            res.IsSuccessful = true;
            res.Result = true;
            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            return res;
            
        }

       
    }
}
