using BLL;
using BLL.Constants;
using BLL.EmailServices;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Authentication;
using Contracts.Email;
using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.TeacherServices;
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
        private readonly DataContext context;
        private readonly IEmailService emailService;
        private readonly IPaginationService paginationService;
        public ParentService(IFileUploadService upload, UserManager<AppUser> userManager, DataContext context, IEmailService emailService)
        {
            this.upload = upload;
            this.userManager = userManager;
            this.context = context;
            this.emailService = emailService;
        }
        async Task<APIResponse<UpdateParent>> IParentService.UpdateParent(UpdateParent ParentDetail)
        {
            var res = new APIResponse<UpdateParent>();
            var uploadProfile = upload.UpdateProfileImage(ParentDetail.Photo, ParentDetail.ImagePath);
            var user = await userManager.FindByIdAsync(ParentDetail.ParentId);
            if (user == null)
            {
                res.Message.FriendlyMessage = "Parent user account does not exist";
                return res;
            }
            var ParentAct = context.Parents.FirstOrDefault(d => d.ParentId == user.Id);
            if (ParentAct != null)
            {
                user.Email = ParentDetail.Email;
                user.UserName = ParentDetail.Email;
                user.UpdateOn = DateTime.Now;
                user.FirstName = ParentDetail.FirstName;
                user.LastName = ParentDetail.LastName;
                user.MiddleName = ParentDetail.MiddleName;
                user.Phone = ParentDetail.Phone;
                user.DOB = ParentDetail.DOB;
                user.Photo = uploadProfile;
                user.EmailConfirmed = true;
                var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, ParentDetail.Phone);
                await userManager.ChangePhoneNumberAsync(user, ParentDetail.Phone, token);
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "Successfully updated a staff";
            res.Result = ParentDetail;
            return res;
        }
        async Task<APIResponse<UpdateParent>> IParentService.AddParentAsync(Parents parent)
        {
            var res = new APIResponse<UpdateParent>();
            try
            {
                var uploadProfile = upload.UploadProfileImage(parent.Photo);
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
                    DOB = parent.DOB,
                    FirstName = parent.FirstName,
                    LastName = parent.LastName,
                    MiddleName = parent.MiddleName,
                    Phone = parent.Phone,
                    PhoneNumber = parent.Phone,
                    PhoneNumberConfirmed = false,
                    Photo = uploadProfile
                };
                var result = await userManager.CreateAsync(user, UserConstants.PASSWORD);
                if (!result.Succeeded)
                {
                    res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                    return res;
                }
                var addTorole = await userManager.AddToRoleAsync(user, DefaultRoles.PARENTS);
                if (!addTorole.Succeeded)
                {
                    res.Message.FriendlyMessage = addTorole.Errors.FirstOrDefault().Description;
                    return res;
                }
                context.Parents.Add(new Parents { ParentId = user.Id, Status = (int)TeacherStatus.Active });
                await context.SaveChangesAsync();
                await SendEmailToParentOnCreateAsync(user);
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successfully added a Parent";
                return res;
            }
            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }
        async Task<APIResponse<PagedResponse<List<Parents>>>> IParentService.GetAllParentsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<Parents>>>();
            var query = context.Parents.OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted == false && d.UserType == (int)UserTypes.Parent);
            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(a => new Parents()
                                                                                {
                                                                                    FirstName = a.FirstName,
                                                                                    Email = a.Email,
                                                                                    PhoneNumber = a.PhoneNumber,
                                                                                    Relationship = a.Relationship
                                                                                }).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }
         async Task<APIResponse<List<StudentDTO>>> IParentService.GetAllStudentsByParentId(Parents parent)
        {
            var res = new APIResponse<List<StudentDTO>>();
            res.Result = await context.StudentParent
                                    .Include(d => d.Students)
                                    .ThenInclude(d => d.User)
                                    .Where(d => d.Parents.ParentId == parent.ParentId)
                                    .Select(d=>new StudentDTO()
                                    {
                                        RegistrationNumber = d.Students.Select(d => d.RegistrationNumber).FirstOrDefault(),
                                        FullName = d.Students.Select(d => d.User.FirstName +" " + d.User.LastName).FirstOrDefault(),
                                        
                                    }).ToListAsync();


            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;

        }

        private async Task SendEmailToParentOnCreateAsync(AppUser obj)
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
    }
}