using BLL.Constants;
using Contracts.Authentication;
using Contracts.Common;
using DAL;
using DAL.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<UserRole> manager;
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext context;

        public RolesService(RoleManager<UserRole> manager, UserManager<AppUser> userManager, DataContext context)
        {
            this.manager = manager;
            this.userManager = userManager;
            this.context = context;
        }
        async Task<APIResponse<UserRole>> IRolesService.CreateRoleAsync(CreateRoleActivity request)
        {
            var res = new APIResponse<UserRole>();
            try
            {
                var role = new UserRole
                {
                    Name = request.Name,
                    Active = true,
                    Deleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                };
                var result = await manager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }

                await CreateRoleActivitiesAsync(request.Activities, role.Id);

                res.IsSuccessful = true;
                res.Result = role;
                res.Message.FriendlyMessage = "Successfully created role";
                return res;
               
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        private async Task CreateRoleActivitiesAsync(string[] activities, string roleId)
        {
            var activitiesList = new List<RoleActivity>();
            foreach(var item in activities)
            {
                var roleActivity = new RoleActivity
                {
                    ActivityId = Guid.Parse(item),
                    RoleId = roleId,
                    CanCreate = true,
                    CanDelete = true,
                    CanExport = true,
                    CanImport = true,
                    CanUpdate = true,
                };
                activitiesList.Add(roleActivity);
            }
            await context.RoleActivity.AddRangeAsync(activitiesList);
            await context.SaveChangesAsync();
        }

        async Task<APIResponse<UserRole>> IRolesService.UpdateRoleAsync(UpdateRoleActivity request)
        {
            var res = new APIResponse<UserRole>();
            var role = await manager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                res.Message.FriendlyMessage = "Role does not exist";
                return res;
            }

            if (role.Name == DefaultRoles.FLAVETECH)
            {
                res.Message.FriendlyMessage = "Role cannot be edited ";
                return res;
            }
            role.Name = request.Name;
            role.UpdateOn = DateTime.Now;
            var result = await manager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                return res;
            }

            await DeleteExistingActivitiesAsync(request.RoleId);
            await CreateRoleActivitiesAsync(request.Activities, request.RoleId);

            res.IsSuccessful = true;
            res.Result = role;
            res.Message.FriendlyMessage = "Successfully updated role";
            return res;
        }

        async Task DeleteExistingActivitiesAsync(string roleId)
        {
            
            var existingActivities = await context.RoleActivity.Where(context => context.RoleId == roleId).ToListAsync();  
            if (existingActivities.Any())
                context.RoleActivity.RemoveRange(existingActivities);
            await context.SaveChangesAsync();
        }

        async Task<APIResponse<List<ApplicationRoles>>> IRolesService.GetAllRolesAsync()
        {
            var res = new APIResponse<List<ApplicationRoles>>();
            var result = await manager.Roles.OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted != true && d.Name != DefaultRoles.FLAVETECH)
                .OrderByDescending(we => we.UpdatedBy)
                .Select(a => new ApplicationRoles { RoleId = a.Id, Name = a.Name }).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<List<GetActivities>>> IRolesService.GetAllActivitiesAsync()
        {
            var res = new APIResponse<List<GetActivities>>();
            var result =  await context.AppActivity.OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted != true)
                .OrderByDescending(we => we.UpdatedBy)
                .Select(a => new GetActivities {  
                    ActivityId = a.Id.ToString(), 
                    Name = a.DisplayName, 
                    ParentId = a.ActivityParentId.ToString(), 
                    ParentName = a.Parent.Name
                }).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }
        async Task<APIResponse<List<GetActivityParent>>> IRolesService.GetActivityParentsAsync()
        {
            var res = new APIResponse<List<GetActivityParent>>();
            var result = await context.AppActivityParent.Where(d => d.Deleted != true)
                .Select(a => new GetActivityParent
                {
                    ParentActivityId = a.Id.ToString(),
                    Name = a.Name,
                }).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }


        async Task<APIResponse<GetRoleActivities>> IRolesService.GetSingleRoleAsync(string roleId)
        {
            var res = new APIResponse<GetRoleActivities>();
            var roleActivities = new List<RoleActivities>();

            var allActivities = await context.AppActivity.Where(d => d.Deleted != true)
              .OrderByDescending(we => we.UpdatedBy)
              .Select(a => new RoleActivities
              {
                  ActivityId = a.Id.ToString(),
                  Name = a.Permission,
                  ParentId = a.ActivityParentId.ToString(),
                  ParentName = a.Parent.Name
              }).ToListAsync();

            var role = await context.Roles.Where(d => d.Id == roleId).Select(w => new GetRoleActivities
            {
                Name = w.Name,
                RoleId = roleId
            }).FirstOrDefaultAsync();
            if(role != null)
            {
                var activities = await context.RoleActivity.Include(d => d.Activity).Include(d => d.UserRole).Where(d => d.RoleId == roleId).Select(a =>
                new RoleActivities
                {
                    ActivityId = a.Activity.Id.ToString(),
                    Name = a.Activity.Permission,
                    ParentId = a.Activity.ActivityParentId.ToString(),
                    ParentName = a.Activity.Parent.Name,
                }).ToListAsync();

                roleActivities = activities;
            }

            roleActivities.AddRange(allActivities);

            role.Activities = roleActivities.GroupBy(p => p.Name).Select(grp => grp.First()).ToList();

            res.Result = role;
            res.IsSuccessful = true;
            res.Result = role;
            return res;
        }

        async Task<APIResponse<UserRole>> IRolesService.DeleteRoleAsync(MultipleDelete request)
        {
            var res = new APIResponse<UserRole>();
            foreach(var roleId in request.Items)
            {
                var role = await manager.FindByIdAsync(roleId);
                if (role == null)
                {
                    res.Message.FriendlyMessage = "Role does not exist";
                    return res;
                }

                if (role.Name == DefaultRoles.TEACHER)
                {
                    res.Message.FriendlyMessage = "Teacher role cannot be deleted ";
                    return res;
                }

                if (role.Name == DefaultRoles.STUDENT)
                {
                    res.Message.FriendlyMessage = "Student role cannot be deleted ";
                    return res;
                }

                if (role.Name == DefaultRoles.SCHOOLADMIN)
                {
                    res.Message.FriendlyMessage = "Admin role cannot be deleted";
                    return res;
                }
                if (role.Name == DefaultRoles.SCHOOLADMIN)
                {
                    res.Message.FriendlyMessage = "Role cannot be deleted";
                    return res;
                }
                role.Deleted = true;
                role.Name = role.Name + "_DELETED";
                var result = await manager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }

                res.Result = role;
            }
          
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully delted role";
            return res;
        }

    }
}
