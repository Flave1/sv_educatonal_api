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

        async Task<APIResponse<AnnouncementsContract>> IAnnouncementsService.UpdateAnnouncementsAsync(AnnouncementsContract request)
        {
            var res = new APIResponse<AnnouncementsContract>();
            var announcement = await context.Announcement.FirstOrDefaultAsync(x=>x.AnnouncementsId == request.AnnouncementsId);
            if(announcement != null)
            { 
                announcement.SeenById = new Guid();
                announcement.Subject = request.Subject;
                announcement.AnnouncementDate = DateTime.UtcNow;
                announcement.AssignedTo = request.AssignedTo;
                announcement.AssignedBy = request.AssignedBy;
                announcement.Body = request.Body;

                await context.SaveChangesAsync();
            }
            else
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }
            res.Message.FriendlyMessage = Messages.Created;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }

        async Task<APIResponse<List<GetAnnouncementsContract>>> IAnnouncementsService.GetAnnouncementsAsync()
        { 
            var res = new APIResponse<List<GetAnnouncementsContract>>();
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var announcements = await context.Announcement.Select(x => new GetAnnouncementsContract(x)).ToListAsync();
            if (!string.IsNullOrEmpty(userid))
            {
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    announcements.Any();
                }
                if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
                {
                    announcements = await context.Announcement.Where(x=>x.SubjectTeacherId == Guid.Parse(teacherId)).Select(f=>new GetAnnouncementsContract(f)).ToListAsync();
                } 

            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = announcements;
            return res;

        }

        async Task<APIResponse<AnnouncementsContract>> IAnnouncementsService.CreateAnnouncementsAsync(AnnouncementsContract request)
        {
            var res = new APIResponse<AnnouncementsContract>();
            if(request != null)
            {
                var newAnnouncement = new Announcements()
                {
                    //todo: getseenby
                    SeenById = new Guid(),
                    Subject = request.Subject,
                    AnnouncementDate = DateTime.UtcNow,
                    AssignedTo = request.AssignedTo,
                    AssignedBy = request.AssignedBy,
                    Body = request.Body
                };
                await context.Announcement.AddAsync(newAnnouncement);
                await context.SaveChangesAsync();
            }
            else
            {
                res.Message.FriendlyMessage = "Make some Announcement";
                return res;
            }
            res.Message.FriendlyMessage = Messages.Created;
            res.IsSuccessful = true;
            res.Result = request;
            return res;
        }  
         
    }
}
