using BLL;
using BLL.Constants;
using BLL.EmailServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Email;
using DAL;
using DAL.Authentication;
using DAL.TeachersInfor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TeacherServices
{
    public class TeacherService : ITeacherService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext context;
        private readonly IEmailService emailService;

        public TeacherService(UserManager<AppUser> userManager, DataContext context, IEmailService emailService)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailService = emailService;
        }

        async Task<APIResponse<UserCommand>> ITeacherService.CreateTeacherAsync(UserCommand request)
        {
            var res = new APIResponse<UserCommand>();
            if (userManager.Users.Any(e => e.Email.ToLower().Trim().Contains(request.Email.ToLower().Trim())))
            {
                res.Message.FriendlyMessage = "Teacher With Email Has Already been Added";
                return res;
            }
            var user = new AppUser
            {
                UserName = request.Email,
                Active = true,
                Deleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "",
                Email = request.Email,
                UserType = (int)UserTypes.Teacher,
                EmailConfirmed = false,
                DOB = request.DOB,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Phone = request.Phone,
                PhoneNumber = request.Phone,
                PhoneNumberConfirmed = false,
                Photo = null,
            };
            var result = await userManager.CreateAsync(user, UserConstants.PASSWORD);
            if (!result.Succeeded)
            {
                res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                return res;
            }
            var addTorole = await userManager.AddToRoleAsync(user, DefaultRoles.TEACHER);
            if (!addTorole.Succeeded)
            {
                res.Message.FriendlyMessage = addTorole.Errors.FirstOrDefault().Description;
                return res;
            }

            context.Teacher.Add(new Teacher { UserId = user.Id, Status = (int)TeacherStatus.Active });
            await context.SaveChangesAsync();

            //await SendEmailToTeacherOnCreateAsync(user);
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully added a staff";
            res.Result = request;
            return res;
        }

        private async Task SendEmailToTeacherOnCreateAsync(AppUser obj)
        {
            var to = new List<EmailAddress>();
            var frm = new List<EmailAddress>();
            to.Add(new EmailAddress { Address = obj.Email, Name = obj.UserName });
            var emMsg = new EmailMessage
            {
                Content = $"Hello <br>" +
                $"Your email {obj.UserName} has been added to the school database <br>" +
                $"Please loggin to your account to verify email. <br>" +
                $"Email: {obj.UserName} <br>" +
                $"Password: {UserConstants.PASSWORD}",
                SentBy = "Flavetechs",
                Subject = "Account Verification",
                ToAddresses = to,
                FromAddresses = frm
            };
            await emailService.Send(emMsg);
        }

        async Task<APIResponse<UserCommand>> ITeacherService.UpdateTeacherAsync(UserCommand userDetail)
        {
            var res = new APIResponse<UserCommand>();
            var user = await userManager.FindByIdAsync(userDetail.TeacherUserAccountId);
            if (user == null)
            {
                res.Message.FriendlyMessage = "Teacher user account does not exist";
                return res;
            }
            var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id);
            if (teacherAct != null)
            {
                user.Email = userDetail.Email;
                user.UserName = userDetail.Email;
                user.UpdateOn = DateTime.Now;
                user.FirstName = userDetail.FirstName;
                user.LastName = userDetail.LastName;
                user.MiddleName = userDetail.MiddleName;
                user.Phone = userDetail.Phone;
                user.DOB = userDetail.DOB;
                user.Photo = userDetail.Photo;
                user.EmailConfirmed = true;
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully updated a staff";
            res.Result = userDetail;
            return res;
        }

        async Task<APIResponse<List<ApplicationUser>>> ITeacherService.GetAllTeachersAsync()
        {
            var res = new APIResponse<List<ApplicationUser>>();
            var result = await context.Teacher.OrderByDescending(d => d.CreatedOn).Include(s => s.User)
                .Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher).Select(a => new ApplicationUser(a)).ToListAsync();
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<ApplicationUser>>> ITeacherService.GetAllActiveTeachersAsync()
        {
            var res = new APIResponse<List<ApplicationUser>>();

            var result = await context.Teacher.OrderByDescending(d => d.CreatedOn).Include(s => s.User)
                .Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher && d.Status == (int)StudentStatus.Active).Select(a => new ApplicationUser(a)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ApplicationUser>> ITeacherService.GetSingleTeacherAsync(Guid teacherId)
        {
            var res = new APIResponse<ApplicationUser>();
            var result = await context.Teacher.OrderByDescending(d => d.CreatedOn).Include(s => s.User)
                .Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher && d.TeacherId == teacherId).Select(a => new ApplicationUser(a)).FirstOrDefaultAsync();
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> ITeacherService.DeleteTeacherAsync(MultipleDelete request)
        {
            var res = new APIResponse<bool>();
          
            foreach(var id in request.Items)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    res.Message.FriendlyMessage = "Staff  does not exist";
                    return res;
                }

                var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id);
                if (teacherAct != null)
                {
                    teacherAct.Deleted = true;
                    user.Deleted = true;
                    user.Active = false;
                    user.Email = "DELETE" + user.Email;
                    user.UserName = "DELETE" + user.UserName;
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                        return res;
                    }
                }
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.Result = true;
            res.IsSuccessful = true;
            return res;

        }
    }
}
