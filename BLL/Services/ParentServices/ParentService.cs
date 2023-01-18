﻿using BLL;
using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.Utilities;
using BLL.Wrappers;
using Contracts.Annoucements;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.ParentModels;
using SMP.DAL.Migrations;
using SMP.DAL.Models.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public class ParentService : IParentService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly IUserService userService;

        public ParentService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService, IUserService userService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.userService = userService;
        }

        async Task<Guid> IParentService.SaveParentDetail(string email, string name, string relationship, string number, Guid id)
        {
            string userid = string.Empty;
            var parent = context.Parents.FirstOrDefault(x => x.Email.ToLower() == email.Trim().ToLower()) ?? null;
            if (parent == null)
                parent = context.Parents.FirstOrDefault(x => x.Parentid == id) ?? null;

            if (parent == null)
            {
                userid = await userService.CreateParentUserAccountAsync(email, number);
                parent = new Parents();
                parent.Name = name;
                parent.Relationship = relationship;
                parent.Number = number;
                parent.Email = email.Trim();
                parent.UserId = userid;
                context.Parents.Add(parent);
            }
            else
            {
                parent.Name = name;
                parent.Relationship = relationship;
                parent.Number = number;
                parent.Email = email.Trim();
                await userService.UpdateParentUserAccountAsync(email, number, parent.UserId);
            }
            context.SaveChanges();
            return parent.Parentid;
        }

        async Task<APIResponse<PagedResponse<List<MyWards>>>> IParentService.GetMyWardsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<MyWards>>>();
            var userName = accessor.HttpContext.User.FindFirst(e => e.Type == "userName")?.Value;

            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            if (!string.IsNullOrEmpty(userName))
            {
                var query = context.StudentContact
                    .Include(x => x.Parent)
                    .Where(x => x.Parent.Email == userName)
                        .Include(d => d.User)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Class)
                        .OrderByDescending(d => d.User.FirstName)
                        .Where(d => d.Deleted == false);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new MyWards(x, regNoFormat)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;

        }

        public async Task<APIResponse<PagedResponse<List<GetAnnouncements>>>> GetAnnouncementsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetAnnouncements>>>();
            try
            {
                var userName = accessor.HttpContext.User.FindFirst(e => e.Type == "userName")?.Value;

                var parent = await context.Parents.FirstOrDefaultAsync(x => x.Email.ToLower() == userName.ToLower());
                var query = context.Announcement
                        .Include(d => d.Sender)
                    .OrderByDescending(d => d.CreatedOn)
                    .Where(d => d.AssignedTo.ToLower() == "parent" && d.Deleted == false);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetAnnouncements(x, parent.UserId)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<GetAnnouncements>> GetAnnouncementDetailsAsync(string announcementId)
        {
            var res = new APIResponse<GetAnnouncements>();
            try
            {
                var userName = accessor.HttpContext.User.FindFirst(e => e.Type == "userName")?.Value;

                var parent = await context.Parents.FirstOrDefaultAsync(x => x.Email.ToLower() == userName.ToLower());
                var result = await context.Announcement
                            .Include(d => d.Sender)
                            .OrderByDescending(d => d.CreatedOn)
                            .Where(d => d.AssignedTo.ToLower() == "parent" && d.AnnouncementsId == Guid.Parse(announcementId) && d.Deleted == false)
                            .Select(x => new GetAnnouncements(x, parent.UserId)).FirstOrDefaultAsync();

                res.Result = result;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<GetAnnouncements>> UpdateSeenAnnouncementAsync(UpdatSeenAnnouncement request)
        {

            var res = new APIResponse<GetAnnouncements>();
            try
            {
                var userName = accessor.HttpContext.User.FindFirst(e => e.Type == "userName")?.Value;

                var parent = await context.Parents.FirstOrDefaultAsync(x => x.Email.ToLower() == userName.ToLower());
                var announcement = await context.Announcement.Include(d => d.Sender).FirstOrDefaultAsync(x => x.AnnouncementsId == Guid.Parse(request.AnnouncementsId));
                if (announcement != null)
                {
                    var splitedIds = !string.IsNullOrEmpty(announcement.SeenByIds) ? announcement.SeenByIds.Split(',').ToList() : new List<string>();
                    if (!splitedIds.Any(d => d == parent.UserId))
                    {
                        splitedIds.Add(parent.UserId);
                        announcement.SeenByIds = string.Join(',', splitedIds);
                        await context.SaveChangesAsync();

                    }
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.Result = new GetAnnouncements(announcement, parent.UserId);
                    res.IsSuccessful = true;

                    return res;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }
            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<GetParents>>>> GetParentsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetParents>>>();
            try
            {
                var query = context.Parents
                    .OrderBy(d => d.Name);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetParents(x)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<GetParentWards>>>> GetParentWardsAsync(PaginationFilter filter, string parentId)
        {

            var res = new APIResponse<PagedResponse<List<GetParentWards>>>();

            try
            {
                var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
                var query = context.StudentContact
                        .Include(x => x.Parent)
                        .Where(x => x.ParentId == Guid.Parse(parentId))
                        .Include(d => d.User)
                        .Include(x => x.SessionClass).ThenInclude(x => x.Class)
                        .OrderByDescending(d => d.User.FirstName)
                        .Where(d => d.Deleted == false);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(x => new GetParentWards(x, regNoFormat)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<GetParents>> GetParentByIdAsync(string parentId)
        {
            var res = new APIResponse<GetParents>();
            try
            {
                var parent = await context.Parents
                    .Where(d => d.Parentid == Guid.Parse(parentId)).Select(parent => new GetParents(parent)).FirstOrDefaultAsync();

                res.Result = parent;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
