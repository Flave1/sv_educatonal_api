using Contracts.Authentication;
using DAL.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using BLL.Constants;
using BLL.EmailServices;
using Contracts.Email;
using DAL;
using DAL.TeachersInfor;
using Contracts.Options;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace BLL.AuthenticationServices
{
    public class UserService  : IUserService
    {
        private readonly UserManager<AppUser> manager;
        private readonly IEmailService emailService;
        private readonly RoleManager<UserRole> roleManager;
        private readonly DataContext context;
        private readonly IIdentityService identityService;
        private readonly SchoolSettings schoolSettings;
        public UserService(UserManager<AppUser> manager, IEmailService emailService, RoleManager<UserRole> roleManager, DataContext context, IIdentityService identityService, IOptions<SchoolSettings> options)
        {
            this.manager = manager;
            this.emailService = emailService;
            this.roleManager = roleManager;
            this.context = context;
            this.identityService = identityService;
            this.schoolSettings = options.Value;
        }

        async Task IUserService.AddUserToRoleAsync(string roleId, AppUser user, string[] userIds)
        {
            if(user == null)
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                    throw new ArgumentException("Role does not exist");

                if (userIds.Any())
                {
                    foreach(var userId in userIds)
                    {
                        user = manager.Users.FirstOrDefault(f => f.Id == userId);
                        if (user == null)
                            throw new ArgumentException("User account not found");

                        
                        var result = await manager.AddToRoleAsync(user, role.Name);
                        if (!result.Succeeded)
                            throw new ArgumentException(result.Errors.FirstOrDefault().Description);
                    }
                 
                }
                
            }
            
        }

        async Task IUserService.CreateTeacherAsync(string email)
        {
            if (manager.Users.Any(e => e.Email.ToLower().Trim().Contains(email.ToLower().Trim())))
                throw new ArgumentException("Teacher With Email Has Already been Added");
            var user = new AppUser
            {
                UserName = email, 
                Active = true,
                Deleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "",
                Email = email,
                UserType = (int)UserTypes.Teacher
            };
            var result = await manager.CreateAsync(user, UserConstants.PASSWORD);
            if (!result.Succeeded) 
                 throw new ArgumentException(result.Errors.FirstOrDefault().Description);  
            var addTorole = await manager.AddToRoleAsync(user, DefaultRoles.TEACHER);
            if (!addTorole.Succeeded)
                throw new ArgumentException(addTorole.Errors.FirstOrDefault().Description);

            context.Teacher.Add(new Teacher { UserId = user.Id });
            await context.SaveChangesAsync(); 

            await SendEmailToTeacherOnCreateAsync(user);
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

        async Task IUserService.UpdateTeacherAsync(UpdateTeacher userDetail)
        {
            var user = await manager.FindByIdAsync(userDetail.TeacherUserAccountId);
            if (user == null) 
                throw new ArgumentException("Teacher user account does not exist");
            var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id);
            if(teacherAct != null)
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
                var result = await manager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            } 
        }

        async Task<List<ApplicationUser>> IUserService.GetAllTeachersAsync()
        {
            return await context.Teacher.Include(s => s.User).Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher).Select(a => new ApplicationUser(a)).ToListAsync();
        }

        async Task IUserService.DeleteUserAsync(string UserId)
        {
            var user = await manager.FindByIdAsync(UserId);
            if (user == null)
                throw new ArgumentException("user does not exist");
            var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id);
            if (teacherAct != null)
            {
                teacherAct.Deleted = true;
                user.Deleted = true;
                user.Active = false;
                var result = await manager.UpdateAsync(user);
                if (!result.Succeeded) 
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description); 
            }
            
        }

        async Task<string> IUserService.CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat)
        {
            var email  = !string.IsNullOrEmpty(student.Email) ? student.Email : regNo.Replace("/", "") + "@school.com";
            var user = new AppUser
            {
                UserName = email,
                Active = true,
                Deleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "",
                Email = email,
                UserType = (int)UserTypes.Student,
                LastName = student.LastName,
                DOB = student.DOB,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                Phone = student.Phone
            };
            var result = await manager.CreateAsync(user, regNoFormat);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }
            var addTorole = await manager.AddToRoleAsync(user, DefaultRoles.STUDENT);
            if (!addTorole.Succeeded)
                throw new ArgumentException(addTorole.Errors.FirstOrDefault().Description);

            return user.Id;
        }

        void IUserService.ValidateResetOption(ResetPassword request)
        {
            if (request.ResetOption == "email")
            { 
                if (string.IsNullOrEmpty(request.ResetOptionValue))
                {
                    throw new ArgumentException("Email is required to reset password");
                }
                try
                {
                    MailAddress m = new MailAddress(request.ResetOptionValue);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Email is invalid");
                }
                catch (Exception)
                {

                    throw new ArgumentException("Unexpected error occurred");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.ResetOptionValue))
                {
                    throw new ArgumentException("Phone number is required to reset password");
                }

                throw new ArgumentException("Password reset by phone number is not available please make use of email reset option");
            } 
        }

        async Task IUserService.GenerateResetLinkAndSendToUserEmail(ResetPassword request)
        {
            if(int.Parse(request.UserType) == (int)UserTypes.Student)
            {
                var user = await manager.Users.FirstOrDefaultAsync(d => d.UserType == (int)UserTypes.Student && d.Email.ToLower().Trim() == request.ResetOptionValue.ToLower().Trim());
                if (user == null)
                    throw new ArgumentException("Student account with this email address is not registered");

                var token = await manager.GeneratePasswordResetTokenAsync(user); 
                var link = schoolSettings.Url + "AccountReset?user=" + token.Replace("+", "tokenSpace") + "&id="+ user.Id;

                await SendResetLinkToEmailToUserAsync(user, link);
            }
           
        }

        private async Task SendResetLinkToEmailToUserAsync(AppUser obj, string link)
        {
            var to = new List<EmailAddress>();
            var frm = new List<EmailAddress>();
            to.Add(new EmailAddress { Address = obj.Email, Name = obj.UserName });
            var emMsg = new EmailMessage
            {
                Content = $"Click  <a href='{link}'>here</a> to reset password",
                SentBy = "Flavetechs",
                Subject = "Account Verification",
                ToAddresses = to,
                FromAddresses = frm
            };
            await emailService.Send(emMsg);
        }



        async Task<AuthenticationResult> IUserService.ResetAccountAsync(ResetAccount request)
        {
            var user = await manager.FindByIdAsync(request.UserId);
            request.ResetToken = request.ResetToken.Replace("tokenSpace", "+");
            if (user == null)
                throw new ArgumentException("Ooops!! Account not identified");

            var changePassword = await manager.ResetPasswordAsync(user, request.ResetToken, request.Password);
            if (changePassword.Succeeded)
            {
                await SendResetSuccessEmailToUserAsync(user);
                var loginResult = await identityService.LoginAsync(new LoginCommand { Password = request.Password, UserName = user.UserName });
                if (!string.IsNullOrEmpty(loginResult.Token))
                    return loginResult;
            }
            else
                throw new ArgumentException(changePassword.Errors.FirstOrDefault().Description);
            throw new ArgumentException("Ooops!! Something is wrong some where");
        }

        private async Task SendResetSuccessEmailToUserAsync(AppUser obj)
        {
            var to = new List<EmailAddress>();
            var frm = new List<EmailAddress>();
            to.Add(new EmailAddress { Address = obj.Email, Name = obj.UserName });
            var emMsg = new EmailMessage
            {
                Content = $"Your password has been rest successfuly",
                SentBy = "Flavetechs",
                Subject = "Account Reset ",
                ToAddresses = to,
                FromAddresses = frm
            };
            await emailService.Send(emMsg);
        }


    }
}
