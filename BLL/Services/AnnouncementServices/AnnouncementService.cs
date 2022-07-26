using BLL;
using BLL.Constants;
using Contracts.Annoucements;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.AnnouncementsServices;
using SMP.BLL.Utilities;
using SMP.Contracts.GradeModels;
using SMP.DAL.Models.Annoucement;
using SMP.DAL.Models.GradeEntities;
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

        public AnnouncementService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
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

        async Task<APIResponse<List<GetAnnouncements>>> IAnnouncementsService.GetAnnouncementsAsync()
        { 
            var res = new APIResponse<List<GetAnnouncements>>();
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            
            if (!string.IsNullOrEmpty(userid))
            {
             
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    res.Result = await context.Announcement
                        .Include(d => d.Sender)
                        .OrderByDescending(d => d.CreatedOn)
                        .Take(100)
                        .Where(d => d.AssignedTo == "teacher" && d.Deleted == false)
                        .Select(x => new GetAnnouncements(x, userid)).ToListAsync();   
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.STUDENT))
                {
                    res.Result = await context.Announcement
                          .Include(d => d.Sender)
                        .OrderByDescending(d => d.CreatedOn)
                        .Take(100)
                        .Where(d => d.AssignedTo == "student" && d.Deleted == false)
                        .Select(x => new GetAnnouncements(x, userid)).ToListAsync();
                   
                }
                else if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    res.Result = await context.Announcement
                          .Include(d => d.Sender)
                        .OrderByDescending(d => d.CreatedOn)
                        .Take(100).Where(d=>d.Deleted == false)
                        .Select(x => new GetAnnouncements(x, userid)).ToListAsync();
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

        async Task<APIResponse<bool>> IAnnouncementsService.DeleteAnnouncementsAsync(Guid Id)
        {
            var res = new APIResponse<bool>();
            var result = await context.Announcement.FirstOrDefaultAsync(d => d.AnnouncementsId == Id);
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
