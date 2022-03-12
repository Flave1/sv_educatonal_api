using Contracts.Authentication;
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
        async Task IRolesService.CreateRoleAsync(string roleName)
        {
            var role = new UserRole
            {
                Name = roleName,
                Active = true,
                Deleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "",
            };
            var result = await manager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }
        }

        async Task IRolesService.UpdateRoleAsync(string roleName, string roleId)
        {
            var role = await manager.FindByIdAsync(roleId);
            if(role == null) 
                throw new ArgumentException("Role does not exist");

            if(role == null)
                throw new ArgumentException("Role does not exist");

            if (role.Name == "TEACHER")
                throw new ArgumentException("Teacher role cannot be edited ");

            if (role.Name == "STUDENT")
                throw new ArgumentException("Student role cannot be edited ");

            if (role.Name == "SCHOOL_ADMIN")
                throw new ArgumentException("Admin role cannot be edited ");
            role.Name = roleName;
            role.UpdateOn = DateTime.Now;
            var result = await manager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }
        }
         
        async Task<List<ApplicationRoles>> IRolesService.GetAllRolesAsync()
        {
            return await manager.Roles.Where(d => d.Deleted != true)
                .OrderByDescending(we => we.UpdatedBy)
                .Select(a => new ApplicationRoles { RoleId = a.Id, Name = a.Name }).ToListAsync();
        }

        async Task<ApplicationRoles> IRolesService.GetSingleRoleAsync(string roleId)
        {
            return await context.UserRoles.Where(d => d.RoleId == roleId).Select(a => 
            new ApplicationRoles
            {
                RoleId = a.RoleId, 
                Name = context.Roles.FirstOrDefault(e => e.Id == a.RoleId).Name,
                Users = context.Users.Where(e => e.Id == a.UserId).Select(u => new ApplicationUser
                {
                    UserAccountId = u.Id,
                    Email = u.Email,
                    UserName = u.UserName, 
                    TeacherUserAccountId = null,
                    Verified = false
                }).ToList(),
            }).FirstOrDefaultAsync();
        }

        async Task IRolesService.DeleteRoleAsync(string roleId)
        {
            var role = await manager.FindByIdAsync(roleId);
            if (role == null) 
                throw new ArgumentException("Role does not exist"); 

            if(role.Name == "TEACHER")
                throw new ArgumentException("Teacher role cannot be deleted ");

            if (role.Name == "STUDENT")
                throw new ArgumentException("Student role cannot be deleted ");

            if (role.Name == "SCHOOL_ADMIN")
                throw new ArgumentException("Admin role cannot be deleted ");
            role.Deleted = true;
            role.Name = role.Name +"_DELETED";
            var result = await manager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }
        }

    }
}
