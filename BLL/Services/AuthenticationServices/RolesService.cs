using BLL.Constants;
using BLL.LoggerService;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Email;
using DAL;
using DAL.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.TeacherServices;
using SMP.BLL.Utilities;
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
        private readonly string smsClientId;
        private readonly ILoggerService loggerService;
        private readonly EmailConfiguration emailConfiguration;
        private readonly IUtilitiesService utilitiesService;

        public RolesService(
            RoleManager<UserRole> manager,
            UserManager<AppUser> userManager,
            DataContext context, IHttpContextAccessor accessor,
            ILoggerService loggerService, IUtilitiesService utilitiesService)
        {
            this.manager = manager;
            this.userManager = userManager;
            this.context = context;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.loggerService = loggerService;
            this.utilitiesService = utilitiesService;
        }


        async Task<APIResponse<UserRole>> IRolesService.CreateRoleAsync(CreateRoleActivity request)
        {
            var res = new APIResponse<UserRole>();
            try
            {
                var role = new UserRole
                {
                    Name = request.Name + "" + smsClientId,
                    Active = true,
                    Deleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                    ClientId = smsClientId
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
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        private async Task CreateRoleActivitiesAsync(string[] activities, string roleId)
        {
            var activitiesList = new List<RoleActivity>();
            foreach (var item in activities)
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
            try
            {
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
                role.Name = request.Name + "" + smsClientId;
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
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        async Task DeleteExistingActivitiesAsync(string roleId)
        {

            var existingActivities = await context.RoleActivity.Where(context => context.RoleId == roleId && context.ClientId == smsClientId).ToListAsync();
            if (existingActivities.Any())
                context.RoleActivity.RemoveRange(existingActivities);
            await context.SaveChangesAsync();
        }


        

        async Task<APIResponse<List<ApplicationRoles>>> IRolesService.GetAllRolesAsync()
        {
            var res = new APIResponse<List<ApplicationRoles>>();
            try
            {
                var result = await manager.Roles.Where(x => x.Deleted == false).OrderByDescending(d => d.CreatedOn)
                .Where(d => d.Name != DefaultRoles.FLAVETECH
                    && (d.Name == DefaultRoles.AdminRole(smsClientId) || d.Name == DefaultRoles.TeacherRole(smsClientId) || d.Name == DefaultRoles.ParentRole(smsClientId))
                    || d.ClientId == smsClientId)
                .OrderByDescending(we => we.UpdatedBy)
                .Select(a => new ApplicationRoles { RoleId = a.Id, Name = a.Name.Replace(smsClientId, "") }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        async Task<APIResponse<List<GetActivities>>> IRolesService.GetAllActivitiesAsync()
        {
            var res = new APIResponse<List<GetActivities>>();
            try
            {
                var result = await context.AppActivity.Where(d => d.Deleted != true).OrderByDescending(d => d.CreatedOn)
                    .OrderByDescending(we => we.UpdatedBy)
                    .Select(a => new GetActivities
                    {
                        ActivityId = a.Id.ToString(),
                        Name = a.DisplayName,
                        ParentId = a.ActivityParentId.ToString(),
                        ParentName = a.Parent.Name
                    }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }
        async Task<APIResponse<List<GetActivityParent>>> IRolesService.GetActivityParentsAsync()
        {
            var res = new APIResponse<List<GetActivityParent>>();
            try
            {
                var result = await context.AppActivityParent.Where(d => d.Deleted != true)
               .Select(a => new GetActivityParent
               {
                   ParentActivityId = a.Id.ToString(),
                   Name = a.Name,
                   DisplayName = a.DisplayName,
               }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }


        async Task<APIResponse<GetRoleActivities>> IRolesService.GetSingleRoleAsync(string roleId)
        {
            var res = new APIResponse<GetRoleActivities>();
            try
            {
                var role = await context.Roles.Where(d => d.Id == roleId).Select(w => new GetRoleActivities
                {
                    Name = w.Name.Replace(smsClientId, ""),
                    RoleId = roleId
                }).FirstOrDefaultAsync();
                if (role != null)
                {
                    role.Activities = context.RoleActivity.Where(d => d.RoleId == roleId && d.ClientId == smsClientId).Select(a => a.ActivityId).ToList();
                }
                res.Result = role;
                res.IsSuccessful = true;
                res.Result = role;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }
        async Task<APIResponse<NotAddedUserRole>> IRolesService.GetUsersNotInRoleAsync(string roleId)
        {
            var res = new APIResponse<NotAddedUserRole>();
            try
            {
                var role = await context.Roles.Where(d => d.Id == roleId).Select(w => new NotAddedUserRole
                {
                    RoleName = w.Name.Replace(smsClientId, ""),
                    RoleId = roleId
                }).FirstOrDefaultAsync();
                if (role != null)
                {
                    var userIds = context.UserRoles.Where(d => d.RoleId == roleId).Select(d => d.UserId);
                    role.Users = context.Teacher.Where(x => !userIds.Contains(x.UserId) && x.ClientId == smsClientId).Select(a => new UserNames(a, null)).ToList();
                }
                res.Result = role;
                res.IsSuccessful = true;
                res.Result = role;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        async Task<APIResponse<UserRole>> IRolesService.DeleteRoleAsync(MultipleDelete request)
        {
            var res = new APIResponse<UserRole>();
            try
            {
                foreach (var roleId in request.Items)
                {
                    var role = await manager.FindByIdAsync(roleId);
                    if (role == null)
                    {
                        res.Message.FriendlyMessage = "Role does not exist";
                        return res;
                    }

                    if (role.Name == DefaultRoles.TeacherRole(smsClientId))
                    {
                        res.Message.FriendlyMessage = "Teacher role cannot be deleted ";
                        return res;
                    }

                    if (role.Name == DefaultRoles.StudentRole(smsClientId))
                    {
                        res.Message.FriendlyMessage = "Student role cannot be deleted ";
                        return res;
                    }

                    if (role.Name == DefaultRoles.AdminRole(smsClientId))
                    {
                        res.Message.FriendlyMessage = "Admin role cannot be deleted";
                        return res;
                    }
                    if (role.Name == DefaultRoles.AdminRole(smsClientId))
                    {
                        res.Message.FriendlyMessage = "Role cannot be deleted";
                        return res;
                    }
                    role.Deleted = true;
                    role.Name = role.Name + "_DELETED" + DateTime.UtcNow.ToUniversalTime();
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
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        async Task<APIResponse<GetUsersInRole>> IRolesService.GetUsersInRoleAsync(GetUsersInRoleRequest request)
        {
            var res = new APIResponse<GetUsersInRole>();
            try
            {
                var userIds = context.UserRoles.Where(d => d.RoleId == request.RoleId).Select(x => x.UserId);
                var selectedRole = context.Roles.Where(d => d.Id == request.RoleId).Select(d => new GetUsersInRole(d, smsClientId)).FirstOrDefault();
                if (selectedRole != null)
                {
                    selectedRole.Users = await context.Teacher.Where(x => userIds.Contains(x.UserId) && x.ClientId == smsClientId).Select(x => new UserNames(x, null)).ToListAsync();

                    res.IsSuccessful = true;
                    res.Result = selectedRole;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }

        async Task<APIResponse<bool>> IRolesService.RemoveUserFromRoleAsync(RemoveUserFromRoleRequest request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var user = await userManager.FindByIdAsync(request.UserId);
                if (user != null)
                {
                    var role = manager.Roles.FirstOrDefault(d => d.Id == request.RoleId);
                    if (role != null)
                    {
                        if (role.Name.StartsWith(DefaultRoles.SCHOOLADMIN))
                            user.UserTypes = utilitiesService.RemoveUserType(user.UserTypes, UserTypes.Admin);
                        if (role.Name.StartsWith(DefaultRoles.TEACHER))
                            user.UserTypes = utilitiesService.RemoveUserType(user.UserTypes, UserTypes.Teacher);
                        if (role.Name.StartsWith(DefaultRoles.STUDENT))
                            user.UserTypes = utilitiesService.RemoveUserType(user.UserTypes, UserTypes.Teacher);

                        var removeResult = await userManager.RemoveFromRoleAsync(user, role.Name);
                        if (removeResult.Succeeded)
                        {
                            res.IsSuccessful = true;
                            res.Result = true;
                            res.Message.FriendlyMessage = "Successfully removed User from role";
                            return res;
                        }
                        else
                        {
                            res.Message.FriendlyMessage = removeResult.Errors.FirstOrDefault().Description;
                            return res;
                        }
                    }
                    else
                    {
                        res.Message.FriendlyMessage = "Role not found";
                        return res;
                    }
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Unable to create Role with activities!! please contact administrator";
                return res;
            }
        }
    }
}
