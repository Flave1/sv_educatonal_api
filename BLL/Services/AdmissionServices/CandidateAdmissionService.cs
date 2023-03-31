using BLL;
using BLL.Constants;
using BLL.EmailServices;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Common;
using Contracts.Email;
using DAL;
using DAL.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Filters;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.PortalService;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using SMP.Contracts.PortalSettings;
using SMP.DAL.Migrations;
using SMP.DAL.Models.Admission;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public class CandidateAdmissionService : ICandidateAdmissionService
    {
        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor accessor;
        private readonly IFileUploadService fileUpload;
        private readonly IPaginationService paginationService;
        private readonly IWebHostEnvironment environment;
        private readonly ILoggerService loggerService;
        private readonly IParentService parentService;
        private readonly UserManager<AppUser> userManager;
        private readonly EmailConfiguration emailConfiguration;
        private readonly string smsClientId;

        public CandidateAdmissionService(DataContext context, IConfiguration config, IEmailService emailService, IOptions<EmailConfiguration> emailOptions,
            IHttpContextAccessor accessor, IFileUploadService fileUpload, IPaginationService paginationService,
            IWebHostEnvironment environment, ILoggerService loggerService, IParentService parentService, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.config = config;
            this.emailService = emailService;
            this.accessor = accessor;
            this.fileUpload = fileUpload;
            this.paginationService = paginationService;
            this.environment = environment;
            this.loggerService = loggerService;
            this.parentService = parentService;
            this.userManager = userManager;
            emailConfiguration = emailOptions.Value;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        public async Task<APIResponse<bool>> ConfirmEmail(ConfirmEmail request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var parent = await context.Parents.Where(d => d.Deleted != true && d.Parentid == Guid.Parse(request.parentId)).FirstOrDefaultAsync();
                if (parent == null)
                {
                    res.Message.FriendlyMessage = "Record does not exist";
                    res.IsSuccessful = false;
                    return res;
                }
                accessor.HttpContext.Items["smsClientId"] = parent.ClientId;
                var user = await userManager.FindByIdAsync(parent.UserId);
                if (user.EmailConfirmed)
                {
                    res.Message.FriendlyMessage = "Your email has already been confirmed. Please proceed to login.";
                    res.IsSuccessful = true;
                    return res;
                }

                user.EmailConfirmed = true;
                var result = await userManager.UpdateAsync(user);
                if(!result.Succeeded)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Email could not be confirmed";
                    return res;
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Email successfully confirmed. Please proceed to login.";
                res.Result = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        public async Task<APIResponse<string>> CreateAdmission(CreateAdmission request)
        {
            var res = new APIResponse<string>();

            try
            {
                var parentId = Guid.Parse(accessor.HttpContext.User.FindFirst(x => x.Type == "parentId").Value);
                var admissionSettings = await context.AdmissionSettings.FirstOrDefaultAsync(x => x.ClientId == smsClientId &&  x.AdmissionStatus == true);

                var filePath = fileUpload.UploadAdmissionCredentials(request.Credentials);
                var photoPath = fileUpload.UploadAdmissionPassport(request.Photo);
                var admission = new DAL.Models.Admission.Admission
                {
                    Firstname = request.Firstname,
                    Middlename = request.Middlename,
                    Lastname = request.Lastname,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    CountryOfOrigin = request.CountryOfOrigin,
                    StateOfOrigin = request.StateOfOrigin,
                    LGAOfOrigin = request.LGAOfOrigin,
                    Credentials = filePath,
                    Photo = photoPath,
                    CandidateAdmissionStatus = (int)CandidateAdmissionStatus.Pending,
                    CandidateCategory = string.Empty,
                    ExaminationStatus = (int)AdmissionExaminationStatus.Pending,
                    ClassId = Guid.Parse(request.ClassId),
                    ParentId = parentId,
                    AdmissionSettingId = admissionSettings.AdmissionSettingId

                };
                context.Admissions.Add(admission);
                await context.SaveChangesAsync();
                res.Result = admission.AdmissionId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch(ArgumentException ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<SelectCandidateAdmission>>>> GetAllAdmission(PaginationFilter filter, string admissionSettingsId)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidateAdmission>>>();
            try
            {
                var parentId = Guid.Parse(accessor.HttpContext.User.FindFirst(x => x.Type == "parentId").Value);

                if(string.IsNullOrEmpty(admissionSettingsId))
                {
                    var admissionSettings = await context.AdmissionSettings.FirstOrDefaultAsync(x => x.ClientId == smsClientId && x.AdmissionStatus == true);
                    
                    if(admissionSettings is null)
                    {
                        res.Message.FriendlyMessage = "Our school admission is not yet open";
                        return res;
                    }
                        var query = context.Admissions
                        .Where(c => c.Deleted != true && c.ParentId == parentId )
                        .Include(c => c.AdmissionSettings)
                        .OrderByDescending(c => c.CreatedOn)
                        .Where(c => c.AdmissionSettingId == admissionSettings.AdmissionSettingId);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                else
                {
                    var query = context.Admissions
                        .Where(c => c.ClientId == smsClientId && c.Deleted != true && c.ParentId == parentId)
                        .Include(c => c.AdmissionSettings)
                        .OrderByDescending(c => c.CreatedOn)
                        .Where(c => c.AdmissionSettingId == Guid.Parse(admissionSettingsId));

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SelectCandidateAdmission>> GetAdmission(string admissionId)
        {
            var res = new APIResponse<SelectCandidateAdmission>();
            try
            {
                var parentId = Guid.Parse(accessor.HttpContext.User.FindFirst(x => x.Type == "parentId").Value);
                var result = await context.Admissions
                    .Where(c => c.ClientId == smsClientId && c.Deleted != true && c.AdmissionId == Guid.Parse(admissionId) && c.ParentId == parentId)
                    .Include(x => x.AdmissionSettings)
                    .Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault(), context.Parents.Where(x => x.Parentid == d.ParentId).FirstOrDefault())).FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        public async Task<APIResponse<bool>> DeleteAdmission(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var parentId = Guid.Parse(accessor.HttpContext.User.FindFirst(x => x.Type == "parentId").Value);
                var admission = await context.Admissions.Where(d => d.Deleted != true && d.AdmissionId == Guid.Parse(request.Item) && d.ParentId == parentId).FirstOrDefaultAsync();
                if (admission == null)
                {
                    res.Message.FriendlyMessage = "Admission Id does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                admission.Deleted = true;
                await context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                res.Result = true;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        private async Task SendNotifications(string parentId, string firstname, string receiver, string schoolURL, SchoolSetting schoolSetting)
        {
            var toEmail = new List<EmailAddress>();
            var frmEmail = new List<EmailAddress>();
            toEmail.Add(new EmailAddress { Address = receiver, Name = receiver });
            frmEmail.Add(new EmailAddress { Address = emailConfiguration.SmtpUsername, Name = schoolSetting.SCHOOLSETTINGS_SchoolName });
            var host = accessor.HttpContext.Request.Host.ToUriComponent();
            string body = $"You've Successfully registered. Kindly click the link below to confirm your email address. <br /> <b>Default Password:</b> 000000 <br /> <br />" +
                $"<a style='text-decoration: none; border: none;border-radius: 3px; color: #FFFFFF; background-color: #008CBA; padding: 10px 18px;margin: 4px 2px;' href='{schoolURL}/candidate-admission?user={parentId}'>Click Here</a>";
            var content = await emailService.GetMailBody(firstname, body, schoolSetting.SCHOOLSETTINGS_SchoolAbbreviation);
            var email = new EmailMessage
            {
                Content = content,
                Subject = "Admission Registration Notice",
                ToAddresses = toEmail,
                FromAddresses = frmEmail,
                SentBy = "Flavetechs"
            };
            await emailService.Send(email);
        }

        public async Task<APIResponse<List<AdmissionClasses>>> GetAdmissionClasses()
        {
            var res = new APIResponse<List<AdmissionClasses>>();
            try
            {
                var classes = context.AdmissionSettings?.Where(d => d.ClientId == smsClientId && d.Deleted != true)?.FirstOrDefault()?.Classes?.Split(',').ToList();
                if (classes != null)
                {

                    var result = await context.ClassLookUp
                        .Where(d => d.Deleted != true && classes.Contains(d.ClassLookupId.ToString())).Select(d => new AdmissionClasses { ClassId = d.ClassLookupId.ToString(), ClassName = d.Name })
                        .ToListAsync();

                    if (result == null)
                    {
                        res.Message.FriendlyMessage = "No item found";
                    }
                    else
                    {
                        res.Message.FriendlyMessage = Messages.GetSuccess;
                    }
                    res.Result = result;
                }
                else
                    res.Result = new List<AdmissionClasses>();

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<string>> UpdateAdmission(UpdateAdmission request)
        {
            var res = new APIResponse<string>();
            try
            {
                var parentId = Guid.Parse(accessor.HttpContext.User.FindFirst(x => x.Type == "parentId").Value);
                var admission = await context.Admissions.Where(m => m.AdmissionId == Guid.Parse(request.AdmissionId) && m.ParentId == parentId).FirstOrDefaultAsync();
                if (admission == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AdmissionId doesn't exist";
                    return res;
                }
                var filePath = fileUpload.UploadAdmissionCredentials(request.Credentials);

                var photoPath = fileUpload.UploadAdmissionPassport(request.Photo);

                admission.Firstname = request.Firstname;
                admission.Lastname = request.Lastname;
                admission.Middlename = request.Middlename;
                admission.Email = request.Email;
                admission.PhoneNumber = request.PhoneNumber;
                admission.DateOfBirth = request.DateOfBirth;
                admission.CountryOfOrigin = request.CountryOfOrigin;
                admission.StateOfOrigin = request.StateOfOrigin;
                admission.LGAOfOrigin = request.LGAOfOrigin;
                admission.Credentials = filePath;
                admission.Photo = photoPath;
                admission.ClassId = Guid.Parse(request.ClassId);
                await context.SaveChangesAsync();

                if(!string.IsNullOrEmpty(admission.Credentials))
                {
                    string oldCredentials = admission.Credentials.Split("AdmissionCredentials/")[1];
                    var oldCredentialsPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + "AdmissionCredentials", oldCredentials);
                    fileUpload.DeleteFile(oldCredentialsPath);
                }

                if(!string.IsNullOrEmpty(admission.Photo))
                {
                    string oldPhoto = admission.Photo.Split("AdmissionPassport/")[1];
                    var oldPhotoPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + "AdmissionPassport", oldPhoto);
                    fileUpload.DeleteFile(oldPhotoPath);
                }

                res.Result = admission.AdmissionId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<List<SelectAdmissionSettings>>> GetSettings()
        {
            var res = new APIResponse<List<SelectAdmissionSettings>>();
            try
            {
                var classes = context.AdmissionSettings?.Where(d => d.ClientId == smsClientId && d.Deleted != true)?.FirstOrDefault()?.Classes?.Split(',', StringSplitOptions.None).ToList();

                if (classes is not null)
                {
                    res.Result = await context.AdmissionSettings
                    .Where(d => d.ClientId == smsClientId && d.Deleted != true)
                    .Select(db => new SelectAdmissionSettings(db, context.ClassLookUp.Where(x => classes.Contains(x.ClassLookupId.ToString())).ToList()))
                    .ToListAsync();

                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }

                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SchoolSettingContract>> GetSchollSettingsAsync()
        {
            var res = new APIResponse<SchoolSettingContract>();
            res.Result = await context.SchoolSettings.Where(x => x.ClientId == smsClientId).Select(f => new SchoolSettingContract(f)).FirstOrDefaultAsync() ?? new SchoolSettingContract();
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<string>> RegisterParent(CreateAdmissionParent request)
        {
            var response = new APIResponse<string>();
            try
            {
                var schoolSettings = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl == request.SchoolUrl);

                if (schoolSettings != null)
                {
                    var parent = await context.Parents.FirstOrDefaultAsync(x=>x.Email.ToLower() == request.Email.ToLower() && x.ClientId == schoolSettings.ClientId);
                    if(parent != null)
                    {
                        response.IsSuccessful = false;
                        response.Message.FriendlyMessage = "User already exist. Please login to continue.";
                        return response;
                    }
                    accessor.HttpContext.Items["smsClientId"] = schoolSettings.ClientId;
                    var parentId = await parentService.SaveParentDetail(request.Email, request.Firstname, request.Lastname, request.Relationship, request.PhoneNumber, Guid.Empty);

                    await SendNotifications(parentId.ToString(), request.Firstname, request.Email, request.SchoolUrl, schoolSettings);

                    response.Result = parentId.ToString();
                    response.IsSuccessful = true;
                    response.Message.FriendlyMessage = "Successfully registered. Kindly check your email, a confirmation mail has been sent to you.";
                }
                return response;

            }
            catch(Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                response.IsSuccessful = false;
                response.Message.FriendlyMessage = Messages.FriendlyException;
                response.Message.TechnicalMessage = ex.ToString();
                return response;
            }
        }
    }
}
