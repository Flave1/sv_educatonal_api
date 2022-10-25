using BLL;
using BLL.Constants;
using DAL.Authentication;
using Microsoft.AspNetCore.Identity;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.Contracts.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public class ParentService : IParentService
    {
        private readonly IFileUploadService upload;
        private readonly UserManager<AppUser> userManager;

        public ParentService(IFileUploadService upload, UserManager<AppUser> userManager)
        {
            this.upload = upload;
            this.userManager = userManager;
        }
        async Task<APIResponse<UpdateParent>> IParentService.UpdateParent(UpdateParent ParentDetail)
        {
                var res = new APIResponse<UpdateParent>();
                var uploadProfile = upload.UpdateProfileImage(userDetail.ProfileImage, userDetail.Photo);
                var user = await userManager.FindByIdAsync(userDetail.TeacherUserAccountId);
                if (user == null)
                {
                    res.Message.FriendlyMessage = "Parent user account does not exist";
                    return res;
                }
                //var teacherAct = context.Teacher.FirstOrDefault(d => d.UserId == user.Id);
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
                    user.Photo = uploadProfile;
                    user.EmailConfirmed = true;

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

        async Task<APIResponse<UpdateParent>> IParentService.AddParentAsync(Parents parent)
        {
            var res = new APIResponse<UpdateParent>();
            try
            {
                var uploadProfile = upload.UploadProfileImage(parent.photo);
                if (userManager.Users.Any(e => e.Email.ToLower().Trim().Contains(parent.Email.ToLower().Trim())))
                {
                    res.Message.FriendlyMessage = "Parent With Email Has Already been Added";
                    return res;
                }
                var user = new AppUser
                {
                    UserName = parent.Email,
                    Active = true,
                    Deleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                    Email = parent.Email,
                    UserType = (int)UserTypes.Parent,
                    EmailConfirmed = false,
                    DOB = parent.Users.DOB,
                    FirstName = parent.Users.FirstName,
                    LastName = parent.Users.LastName,
                    MiddleName = parent.Users.MiddleName,
                    Phone = parent.Users.Phone,
                    PhoneNumber = parent.Users.Phone,
                    PhoneNumberConfirmed = false,
                    Photo = uploadProfile
                };
                var result = await userManager.CreateAsync(user, UserConstants.PASSWORD);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }
                var addTorole = await userManager.AddToRoleAsync(user, DefaultRoles.PARENT);
                if (!addTorole.Succeeded)
                {
                    res.Message.FriendlyMessage = addTorole.Errors.FirstOrDefault().Description;
                    return res;
                }

                /*context.Teacher.Add(new Parents { ParentId = user.Id, Status = (int)TeacherStatus.Active });
                await context.SaveChangesAsync();*//*

                await SendEmailToTeacherOnCreateAsync(user);
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successfully added a Parent";
                res.Result = parent;*/
                return res;

            }
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }

    }
}
