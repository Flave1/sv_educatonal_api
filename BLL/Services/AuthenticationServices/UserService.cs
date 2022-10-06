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
using SMP.Contracts.Options;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Constants;
using Microsoft.AspNetCore.Http;
using SMP.BLL.Services.PinManagementService;
using BLL.Utilities;

namespace BLL.AuthenticationServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> manager;
        private readonly IEmailService emailService;
        private readonly RoleManager<UserRole> roleManager;
        private readonly DataContext context;
        private readonly IIdentityService identityService;
        private readonly SchoolSettings schoolSettings;
        private readonly IFileUploadService uploadService;
        private readonly EmailConfiguration emailConfiguration;
        private readonly FwsConfigSettings fwsConfig;
        private readonly IPinManagementService pinService;
        public UserService(UserManager<AppUser> manager, IEmailService emailService, RoleManager<UserRole> roleManager, DataContext context,
            IIdentityService identityService, IOptions<SchoolSettings> options, IFileUploadService uploadService, IOptions<EmailConfiguration> emailOptions, IOptions<FwsConfigSettings> fwsOptions, IPinManagementService pinService)
        {
            this.manager = manager;
            this.emailService = emailService;
            this.roleManager = roleManager;
            this.context = context;
            this.identityService = identityService;
            this.schoolSettings = options.Value;
            this.uploadService = uploadService;
            emailConfiguration = emailOptions.Value;
            fwsConfig = fwsOptions.Value;
            this.pinService = pinService;
        }

        async Task<APIResponse<string[]>> IUserService.AddUserToRoleAsync(string roleId, AppUser user, string[] userIds)
        {
            var res = new APIResponse<string[]>();
            if (user == null)
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                    throw new ArgumentException("Role does not exist");

                if (userIds.Any())
                {
                    foreach (var userId in userIds)
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
            res.IsSuccessful = true;
            res.Result = userIds;
            res.Message.FriendlyMessage = Messages.Saved;
            return res;

        }



        async Task<string> IUserService.CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat)
        {
            try
            {
                var email = !string.IsNullOrEmpty(student.Email) ? student.Email : regNo.Replace("/", "") + "@school.com";
                var filePath = uploadService.UploadProfileImage(student.ProfileImage);
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
                    if (result.Errors.Select(d => d.Code).Any(a => a == "DuplicateUserName"))
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
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        async Task<string> IUserService.CreateStudentUserAccountAsync(UploadStudentExcel student, string regNo, string regNoFormat)
        {
            try
            {
                var email = !string.IsNullOrEmpty(student.Email) ? student.Email : regNo.Replace("/", "") + "@school.com";
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

                };
                var result = await manager.CreateAsync(user, regNoFormat);
                if (!result.Succeeded)
                {
                    if (result.Errors.Select(d => d.Code).Any(a => a == "DuplicateUserName"))
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
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        async Task IUserService.UpdateStudentUserAccountAsync(StudentContactCommand student)
        {
            try
            {
                var account = await manager.FindByIdAsync(student.UserAccountId);
                if (account == null)
                {
                    throw new ArgumentException("Account not found");
                }

                var filePath = uploadService.UpdateProfileImage(student.ProfileImage, account.Photo);
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
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }

        async Task IUserService.UpdateStudentUserAccountAsync(UploadStudentExcel student, string userAccountId)
        {
            try
            {
                var account = await manager.FindByIdAsync(userAccountId);
                if (account == null)
                {
                    throw new ArgumentException("Account not found");
                }
                account.UserName = student.Email;
                account.Email = student.Email;
                account.UserType = (int)UserTypes.Student;
                account.LastName = student.LastName;
                account.DOB = student.DOB;
                account.FirstName = student.FirstName;
                account.MiddleName = student.MiddleName;
                account.Phone = student.Phone;
                var result = await manager.UpdateAsync(account);
                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }

        async Task IUserService.UpdateStudentUserProfileImageAsync(IFormFile file, string studentId)
        {
            try
            {
                var account = await manager.FindByIdAsync(studentId);
                if (account == null)
                {
                    throw new ArgumentException("Account not found");
                }

                var filePath = uploadService.UpdateProfileImage(file, account.Photo);
              
                account.Photo = filePath;
                var result = await manager.UpdateAsync(account);
                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }

        async Task IUserService.UpdateTeacherUserProfileImageAsync(IFormFile file, AppUser account)
        {
            try
            {
                var filePath = await Task.Run(() => uploadService.UpdateProfileImage(file, account.Photo));
                account.Photo = filePath;
                var result = await manager.UpdateAsync(account);
                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.FirstOrDefault().Description);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
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
            if (int.Parse(request.UserType) == (int)UserTypes.Student)
            {
                var user = await manager.Users.FirstOrDefaultAsync(d => d.UserType == (int)UserTypes.Student && d.Email.ToLower().Trim() == request.ResetOptionValue.ToLower().Trim());
                if (user == null)
                    throw new ArgumentException("Student account with this email address is not registered");

                var token = await manager.GeneratePasswordResetTokenAsync(user);
                var link = schoolSettings.Url + "AccountReset?user=" + token.Replace("+", "tokenSpace") + "&id=" + user.Id;

                await SendResetLinkToEmailToUserAsync(user, link);
            }

        }

        private async Task SendResetLinkToEmailToUserAsync(AppUser obj, string link)
        {
            var to = new List<EmailAddress>();
            var frm = new List<EmailAddress>();
            to.Add(new EmailAddress { Address = obj.Email, Name = obj.UserName });
            frm.Add(new EmailAddress { Address = emailConfiguration.SmtpUsername, Name = emailConfiguration.Sender });
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

        async Task<APIResponse<AuthenticationResult>> IUserService.ResetAccountAsync(ResetAccount request)
        {
            var res = new APIResponse<AuthenticationResult>();
            var user = await manager.FindByIdAsync(request.UserId);
            request.ResetToken = request.ResetToken.Replace("tokenSpace", "+");
            if (user == null)
                throw new ArgumentException("Ooops!! Account not identified");

            var changePassword = await manager.ResetPasswordAsync(user, request.ResetToken, request.Password);
            if (changePassword.Succeeded)
            {
                await SendResetSuccessEmailToUserAsync(user);
                var loginResult = await identityService.WebLoginAsync(new LoginCommand { Password = request.Password, UserName = user.UserName });
                if (!string.IsNullOrEmpty(loginResult.Result.AuthResult.Token))
                    return res;
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
            frm.Add(new EmailAddress { Address = emailConfiguration.SmtpUsername, Name = emailConfiguration.Sender });
            var emMsg = new EmailMessage
            {
                Content = $"Your password has been reset successfuly",
                SentBy = "Flavetechs",
                Subject = "Account Reset",
                ToAddresses = to,
                FromAddresses = frm
            };
            await emailService.Send(emMsg);
        }

        async Task<APIResponse<LoginSuccessResponse>> IUserService.ChangePasswordAsync(ChangePassword request)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            var user = await manager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                res.Message.FriendlyMessage = "Ooops!! Account not identified";
                return res;
            }

            var isUserPAssword = await manager.CheckPasswordAsync(user, request.OldPassword);
            if (!isUserPAssword)
            {
                res.Message.FriendlyMessage = "Old Password incorrect";
                return res;
            }

            var token = await manager.GeneratePasswordResetTokenAsync(user);
            var changePassword = await manager.ResetPasswordAsync(user, token, request.NewPassword);
            if (changePassword.Succeeded)
            {
                await SendResetSuccessEmailToUserAsync(user);
                return await identityService.LoginAfterPasswordIsChangedAsync(user);
            }
            else
            {
                res.Message.FriendlyMessage = changePassword.Errors.FirstOrDefault().Description;
                return res;
            }
        }

        async Task<APIResponse<SmpStudentValidationResponse>> IUserService.ValidateUserInformationFromMobileAsync(UserInformationFromMobileRequest request)
        {
            var res = new APIResponse<SmpStudentValidationResponse>();
            res.Result = new SmpStudentValidationResponse();
            res.IsSuccessful = true;
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            if (request.ClientId.ToLower() != fwsConfig.ClientId.ToLower())
            {
                res.Result.Status = "failed";
                res.Message.FriendlyMessage = "Invalid credentials";
                return res;
            }

            if(request.UserType == (int)UserTypes.Student)
            {

                var rgNo = pinService.GetStudentRealRegNumber(request.UsernameOrRegNumber);
                var student = context.StudentContact.Include(x => x.User).FirstOrDefault(x => x.RegistrationNumber == rgNo);
                if (student is null)
                {
                    res.Result.Status = "failed";
                    res.Message.FriendlyMessage = "Student registration number not identified in selected school";
                    return res;
                }
                else
                {
                    if(request.UsernameOrRegNumber != regNoFormat.Replace("%VALUE%", student.RegistrationNumber))
                    {
                        res.Result.Status = "failed";
                        res.Message.FriendlyMessage = "Student registration number not identified in selected school";
                        return res;
                    }
                    else
                    {
                        res.Result.Status = "success";
                        res.Result.FullName = student.User.FirstName + " " + student.User.LastName;
                        res.Result.RegistrationNumber = student.RegistrationNumber;
                        res.Message.FriendlyMessage = Messages.GetSuccess;
                        res.Result.SchoolLogo = context.SchoolSettings.FirstOrDefault().Photo;
                        return res;
                    }
                }
            }

            else if (request.UserType == (int)UserTypes.Teacher)
            {
                var teacher = await manager.FindByNameAsync(request.UsernameOrRegNumber);
                if(teacher is null)
                {
                    res.Message.FriendlyMessage = "Staff account not identified in selected school";
                    return res;
                }
                else
                {
                    res.Result.Status = "success";
                    res.Result.FullName = teacher.FirstName + " " + teacher.LastName;
                    res.Result.RegistrationNumber = "";
                    res.Result.SchoolLogo = context.SchoolSettings.FirstOrDefault().Photo;
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    return res;
                }
            }

            else if (request.UserType == (int)UserTypes.Admin)
            {
                var teacher = await manager.FindByNameAsync(request.UsernameOrRegNumber);
                if (teacher is null)
                {
                    res.Message.FriendlyMessage = "Staff account not identified in selected school";
                    return res;
                }
                else
                {
                    res.Result.Status = "success";
                    res.Result.FullName = teacher.FirstName + " " + teacher.LastName;
                    res.Result.RegistrationNumber = "";
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                    res.Result.SchoolLogo = context.SchoolSettings.FirstOrDefault().Photo;
                    return res;
                }
            }
            res.Result.Status = "failed";
            res.Message.FriendlyMessage = "invalid request";
            return res;
        }

    }
}
