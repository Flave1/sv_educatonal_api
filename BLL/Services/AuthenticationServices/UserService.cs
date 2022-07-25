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
using System.Data;
using SMP.Contracts.FileUpload;
using SMP.Contracts.Options;

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
        private readonly IFileUploadService uploadService;
        public UserService(UserManager<AppUser> manager, IEmailService emailService, RoleManager<UserRole> roleManager, DataContext context, IIdentityService identityService, IOptions<SchoolSettings> options, IFileUploadService uploadService)
        {
            this.manager = manager;
            this.emailService = emailService;
            this.roleManager = roleManager;
            this.context = context;
            this.identityService = identityService;
            this.schoolSettings = options.Value;
            this.uploadService = uploadService;
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

       

        async Task<string> IUserService.CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat)
        {
            var email  = !string.IsNullOrEmpty(student.Email) ? student.Email : regNo.Replace("/", "") + "@school.com";
            var filePath = uploadService.UploadProfileImageAsync(student.ProfileImage);
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
                Phone = student.Phone,
                Photo = filePath
                
            };
            var result = await manager.CreateAsync(user, regNoFormat);
            if (!result.Succeeded)
            {
                if(result.Errors.Select(d => d.Code).Any(a => a == "DuplicateUserName"))
                {
                    throw new DuplicateNameException(result.Errors.FirstOrDefault().Description);
                }
                else
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }
            var addTorole = await manager.AddToRoleAsync(user, DefaultRoles.STUDENT);
            if (!addTorole.Succeeded)
                throw new ArgumentException(addTorole.Errors.FirstOrDefault().Description);

            return user.Id;
        }

        async Task IUserService.UpdateStudentUserAccountAsync(StudentContactCommand student)
        {
            var account = await manager.FindByIdAsync(student.UserAccountId);
            if(account == null)
            {
                throw new ArgumentException("Account not found");
            }

            var filePath = uploadService.UpdateProfileImageAsync(student.ProfileImage, account.Photo);
            account.UserName = student.Email;
            account.Email = student.Email;
            account.UserType = (int)UserTypes.Student;
            account.LastName = student.LastName;
            account.DOB = student.DOB;
            account.FirstName = student.FirstName;
            account.MiddleName = student.MiddleName;
            account.Phone = student.Phone;
            account.Photo = filePath;
            var result = await manager.UpdateAsync(account);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.FirstOrDefault().Description);
            }

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
