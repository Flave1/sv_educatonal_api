﻿using BLL;
using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.EmailServices;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Email;
using DAL;
using DAL.Authentication;
using DAL.TeachersInfor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.PortalService;
using SMP.Contracts.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TeacherServices
{
    public class TeacherService : ITeacherService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext context;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IFileUploadService upload;
        private readonly IUserService userService;
        private readonly IPaginationService paginationService;
        private readonly string smsClientId;
        private readonly IPortalSettingService portalSettingService;
        private readonly IHttpContextAccessor contextaccessor;
        public TeacherService(UserManager<AppUser> userManager, DataContext context, IEmailService emailService, IWebHostEnvironment environment,
            IFileUploadService upload, IUserService userService, IPaginationService paginationService, IHttpContextAccessor accessor, IPortalSettingService portalSettingService)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailService = emailService;
            this.environment = environment;
            this.upload = upload;
            this.userService = userService;
            this.paginationService = paginationService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.portalSettingService = portalSettingService;
            contextaccessor = accessor;
        }

        async Task<APIResponse<UserCommand>> ITeacherService.CreateTeacherAsync(UserCommand request)
        { 
            var res = new APIResponse<UserCommand>();
            try
            {
                var uploadProfile = upload.UploadProfileImage(request.ProfileImage);
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
                    EmailConfirmed = false
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

                CreateUpdateAdminTeacherProfile(request, user.Id);

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successfully added a staff";
                res.Result = request;
                return res;

            }
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
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
            var uploadProfile = upload.UpdateProfileImage(userDetail.ProfileImage, userDetail.Photo);
            var user = await userManager.FindByIdAsync(userDetail.TeacherUserAccountId);
            if (user == null)
            {
                res.Message.FriendlyMessage = "Teacher user account does not exist";
                return res;
            }
            var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id && d.ClientId == smsClientId);
            if (teacherAct != null)
            {
                CreateUpdateAdminTeacherProfile(userDetail, user.Id);
                user.Email = userDetail.Email;
                user.UserName = userDetail.Email;
                var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, userDetail.Phone);

                await userManager.ChangePhoneNumberAsync(user, userDetail.Phone, token);
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

        async Task<APIResponse<PagedResponse<List<ApplicationUser>>>> ITeacherService.GetAllTeachersAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<ApplicationUser>>>();
            var query = context.Teacher
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher)
                .Include(s => s.User).OrderBy(d => d.FirstName);

            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(a => new ApplicationUser(a)).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<ApplicationUser>>> ITeacherService.GetAllActiveTeachersAsync()
        {
            var res = new APIResponse<List<ApplicationUser>>();

            var result = await context.Teacher
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher && d.Status == (int)TeacherStatus.Active)
                .Include(x => x.User)
                .OrderByDescending(d => d.CreatedOn)
                .Select(a => new ApplicationUser(a)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ApplicationUser>> ITeacherService.GetSingleTeacherAsync(Guid teacherId)
        {
            var res = new APIResponse<ApplicationUser>();
            var result = await context.Teacher
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.User.UserType == (int)UserTypes.Teacher && d.TeacherId == teacherId)
                .Include(x => x.User)
                .OrderByDescending(d => d.CreatedOn)
                .Select(a => new ApplicationUser(a)).FirstOrDefaultAsync();
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

                var teacherAct = context.Teacher.FirstOrDefault(d => d.ClientId == smsClientId && d.UserId == user.Id);
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

        async Task<APIResponse<UpdateProfileByTeacher>> ITeacherService.UpdateTeacherProfileByTeacherAsync(UpdateProfileByTeacher userDetail)
        {
            var res = new APIResponse<UpdateProfileByTeacher>();
            var user = await userManager.FindByIdAsync(userDetail.TeacherUserAccountId);

            if (user == null)
            {
                res.Message.FriendlyMessage = "Teacher user account does not exist";
                return res;
            }

            var teacherAct = context.Teacher.FirstOrDefault(d => d.ClientId == smsClientId && d.UserId == user.Id);

            if (teacherAct != null)
            {
                user.Email = userDetail.Email;
                user.UserName = userDetail.Email;
                user.EmailConfirmed = true;
                teacherAct.Hobbies = string.Join(',', userDetail.Hobbies);
                teacherAct.ShortBiography = userDetail.ShortBiography;
                teacherAct.Address = userDetail.Address;
                teacherAct.Gender = userDetail.Gender;
                teacherAct.MaritalStatus = userDetail.MaritalStatus;
                var filePath = await Task.Run(() => upload.UpdateProfileImage(userDetail.ProfileImage, teacherAct.Photo));
                teacherAct.Photo = filePath;

                var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, userDetail.Phone);

                await userManager.ChangePhoneNumberAsync(user, userDetail.Phone, token);
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

        async Task<APIResponse<TeacheerClassAndSibjects>> ITeacherService.GetSingleTeacherClassesAndSubjectsAsync(Guid teacherId)
        {
            var res = new APIResponse<TeacheerClassAndSibjects>();
            res.Result = new TeacheerClassAndSibjects();
            
            res.Result.ClassesAsFormTeacher = await context.SessionClass
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.FormTeacherId == teacherId && d.Session.IsActive)
                .Include(d => d.Session)
                .Include(s => s.Class).Include(s => s.SessionClassSubjects).ThenInclude(d => d.Subject).OrderByDescending(d => d.Class.Name)
                .Select(a => new TeacherClassesAsFormTeacher
                {
                    Class = a.Class.Name,
                    SubjectsInClass = a.SessionClassSubjects.Select(d => d.Subject.Name).ToList()
                }).ToListAsync();


            res.Result.SubjectsAsSubjectTeacher =  context.SessionClassSubject
                .Include(s => s.Subject)
                .Include(s => s.SessionClass).ThenInclude(d => d.Session)
                .Include(s => s.SessionClass).ThenInclude(d => d.Class)
                .Where(d => d.ClientId == smsClientId && d.Deleted == false && d.SubjectTeacherId == teacherId && d.SessionClass.Session.IsActive)
                .AsEnumerable()
                .GroupBy(s => s.SubjectId).Select(a => new TeacherSubjectsAsSubjectTeacher
                {
                    Subject = a.First().Subject.Name,
                    Class = a.Select(d => d.SessionClass.Class.Name).ToList(),
                }).ToList();


            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<string>> ITeacherService.CreateAdminAsync(UserCommand request)
        {
            var res = new APIResponse<string>();
            try
            {
                contextaccessor.HttpContext.Items["smsClientId"] = request.ClientId;
                if (userManager.Users.Any(e => e.Email.ToLower().Trim().Contains(request.Email.ToLower().Trim())))
                {
                    res.Result = "failed";
                    res.Message.FriendlyMessage = "Teacher With Email Has Already been Added";
                    return res;
                }
                var user = new AppUser();

                user.UserName = request.Email;
                user.Active = true;
                user.Deleted = false;
                user.CreatedOn = DateTime.UtcNow;
                user.CreatedBy = "";
                user.Email = request.Email;
                user.UserType = (int)UserTypes.Admin;
                user.EmailConfirmed = false;
                user.PasswordHash = request.PasswordHash;

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    res.Result = "failed";
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }
                var addTorole = await userManager.AddToRoleAsync(user, DefaultRoles.SCHOOLADMIN);
                if (!addTorole.Succeeded)
                {
                    res.Result = "failed";
                    res.Message.FriendlyMessage = addTorole.Errors.FirstOrDefault().Description;
                    return res;
                }

                CreateUpdateAdminTeacherProfile(request, user.Id);

                var schooSetting = new SMSSMPAccountSetting(request.SchoolName, request.Country, request.State, request.Address, request.SchoolLogo, request.ClientId);

                await portalSettingService.CreateSchoolSettingsAsync(schooSetting, user.Email);

                portalSettingService.CreateAppLayoutSettingsAsync(request.ClientId, request.SchoolUrl);
                await context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successfully added a staff";
                res.Result = "success";
                return res;

            }
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }


        void CreateUpdateAdminTeacherProfile(UserCommand request, string userId)
        {
            try
            {
                var isNew = false;
                var teacher = context.Teacher.FirstOrDefault(x => x.ClientId == smsClientId && userId == x.UserId);
                if (teacher is null)
                {
                    isNew = true;
                    teacher = new Teacher();
                }
                teacher.DOB = request.DOB;
                teacher.FirstName = request.FirstName;
                teacher.LastName = request.LastName;
                teacher.MiddleName = request.MiddleName;
                teacher.Phone = request.Phone;
                teacher.Photo = "";
                teacher.UserId = userId;
                teacher.ClientId = request.ClientId;
                if (isNew) context.Teacher.Add(teacher);
                context.SaveChanges();
            }
            catch (Exception x)
            {

                throw new ArgumentException(x?.Message ?? x?.InnerException.Message);
            }
        }

       

    }
}
