using BLL;
using BLL.EmailServices;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using Contracts.Email;
using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Filters;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using SMP.DAL.Models.Admission;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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
        private readonly EmailConfiguration emailConfiguration;
        private readonly string smsClientId;

        public CandidateAdmissionService(DataContext context, IConfiguration config, IEmailService emailService, IOptions<EmailConfiguration> emailOptions,
            IHttpContextAccessor accessor, IFileUploadService fileUpload, IPaginationService paginationService,
            IWebHostEnvironment environment)
        {
            this.context = context;
            this.config = config;
            this.emailService = emailService;
            this.accessor = accessor;
            this.fileUpload = fileUpload;
            this.paginationService = paginationService;
            this.environment = environment;
            emailConfiguration = emailOptions.Value;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        public async Task<APIResponse<bool>> ConfirmEmail(ConfirmEmail request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var notification = await context.AdmissionNotifications.Where(d => d.Deleted != true && d.AdmissionNotificationId == Guid.Parse(request.AdmissionNotificationId)).FirstOrDefaultAsync();
                if (notification == null)
                {
                    res.Message.FriendlyMessage = "Record does not exist";
                    res.IsSuccessful = false;
                    return res;
                }
                accessor.HttpContext.Items["smsClientId"] = notification.ClientId;
                if (notification.IsConfirmed)
                {
                    res.Message.FriendlyMessage = "Your email has already been confirmed. Please proceed to login.";
                    res.IsSuccessful = true;
                    return res;
                }

                notification.IsConfirmed = true;
                await context.SaveChangesAsync();

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

        public async Task<APIResponse<AdmissionLoginDetails>> Login(AdmissionLogin request)
        {
            var res = new APIResponse<AdmissionLoginDetails>();
            try
            {

                var clientId = context.AppLayoutSetting.FirstOrDefault(x => x.schoolUrl == request.SchoolUrl)?.ClientId?? string.Empty;

                if (!string.IsNullOrEmpty(clientId))
                {
                    accessor.HttpContext.Items["smsClientId"] = clientId;
                    var result = context.AdmissionNotifications.FirstOrDefault(x => x.ClientId == clientId && x.ParentEmail.ToLower() == request.ParentEmail.ToLower());

                    if (result == null)
                    {
                        var notification = new AdmissionNotification
                        {
                            ParentEmail = request.ParentEmail,
                            IsConfirmed = false,
                        };

                        context.AdmissionNotifications.Add(notification);
                        await context.SaveChangesAsync();

                        await SendNotifications(notification.AdmissionNotificationId.ToString(), notification.ParentEmail, request.SchoolUrl);

                        res.Result = new AdmissionLoginDetails(null, new UserDetails(notification.ParentEmail, notification.AdmissionNotificationId.ToString()));

                        res.IsSuccessful = true;
                        res.Message.FriendlyMessage = "Successfully registered. Kindly check your email, a confirmation mail has been sent to you.";
                        return res;
                    }
                    if (result.IsConfirmed == false)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = "Kindly confirm your email address. A confirmation mail was sent to your email.";
                        return res;
                    }

                    res.Result = new AdmissionLoginDetails(await GenerateToken(result.ParentEmail, result.AdmissionNotificationId.ToString(), clientId),
                        new UserDetails(result.ParentEmail, result.AdmissionNotificationId.ToString()));
                    res.IsSuccessful = true;
                    res.Message.FriendlyMessage = "Successful";
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Unable to precess request! Try again later";
                }
                
                return res;



            }
            catch(Exception ex)
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
                var admissionNotificationId = Guid.Parse(accessor.HttpContext.Items["admissionNotificationId"].ToString());
                var admissionSettings = await context.AdmissionSettings.FirstOrDefaultAsync(x => x.ClientId == smsClientId &&  x.AdmissionStatus == true);

                var filePath = fileUpload.UploadAdmissionCredentials(request.Credentials);
                var photoPath = fileUpload.UploadAdmissionPassport(request.Photo);
                var admission = new Admission
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
                    ParentName = request.ParentName,
                    ParentRelationship = request.ParentRelationship,
                    ParentPhoneNumber = request.ParentPhoneNumber,
                    CandidateAdmissionStatus = (int)CandidateAdmissionStatus.Pending,
                    CandidateCategory = string.Empty,
                    ExaminationStatus = (int)AdmissionExaminationStatus.Pending,
                    ClassId = Guid.Parse(request.ClassId),
                    AdmissionNotificationId = admissionNotificationId,
                    AdmissionSettingId = admissionSettings.AdmissionSettingId

                };
                context.Admissions.Add(admission);
                await context.SaveChangesAsync();
                res.Result = admission.AdmissionId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
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

        public async Task<APIResponse<bool>> DeleteEmail(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var notificationEmail = await context.AdmissionNotifications.Where(d => d.ClientId == smsClientId && d.Deleted != true && d.ParentEmail.ToLower() == request.Item.ToLower()).FirstOrDefaultAsync();
                if (notificationEmail == null)
                {
                    res.Message.FriendlyMessage = "Email does not exist";
                    res.IsSuccessful = false;
                    return res;
                }
                context.AdmissionNotifications.Remove(notificationEmail);
                await context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
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
        public async Task<APIResponse<PagedResponse<List<SelectCandidateAdmission>>>> GetAllAdmission(PaginationFilter filter, string admissionSettingsId)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidateAdmission>>>();
            try
            {
                var admissionNotificationId = Guid.Parse(accessor.HttpContext.Items["admissionNotificationId"].ToString());

                if(string.IsNullOrEmpty(admissionSettingsId))
                {
                    var admissionSettings = await context.AdmissionSettings.FirstOrDefaultAsync(x => x.ClientId == smsClientId && x.AdmissionStatus == true);
                    var query = context.Admissions
                        .Where(c => c.Deleted != true && c.AdmissionNotificationId == admissionNotificationId && c.AdmissionSettingId == admissionSettings.AdmissionSettingId)
                        .Include(c => c.AdmissionNotification)
                        .Include(c => c.AdmissionSettings)
                        .OrderByDescending(c => c.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                else
                {
                    var query = context.Admissions
                        .Where(c => c.ClientId == smsClientId && c.Deleted != true && c.AdmissionNotificationId == admissionNotificationId && c.AdmissionSettingId == Guid.Parse(admissionSettingsId))
                        .Include(c => c.AdmissionNotification)
                        .Include(c => c.AdmissionSettings)
                        .OrderByDescending(c => c.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
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

        public async Task<APIResponse<SelectCandidateAdmission>> GetAdmission(string admissionId)
        {
            var res = new APIResponse<SelectCandidateAdmission>();
            try
            {
                var admissionNotificationId = Guid.Parse(accessor.HttpContext.Items["admissionNotificationId"].ToString());
                var result = await context.Admissions
                    .Where(c => c.ClientId == smsClientId && c.Deleted != true && c.AdmissionId == Guid.Parse(admissionId) && c.AdmissionNotificationId == admissionNotificationId)
                    .Include(c => c.AdmissionNotification)
                    .Include(x => x.AdmissionSettings)
                    .Select(d => new SelectCandidateAdmission(d, context.ClassLookUp.Where(x => x.ClassLookupId == d.ClassId).FirstOrDefault())).FirstOrDefaultAsync();

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
                var admissionNotificationId = Guid.Parse(accessor.HttpContext.Items["admissionNotificationId"].ToString());
                var admission = await context.Admissions.Where(d => d.Deleted != true && d.AdmissionId == Guid.Parse(request.Item) && d.AdmissionNotificationId == admissionNotificationId).FirstOrDefaultAsync();
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
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        private async Task<AdmissionAuth> GenerateToken(string parentEmail, string admissionNotificationId, string smsClientId)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, parentEmail),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Email, parentEmail),
               new Claim("parentEmail", parentEmail),
               new Claim("admissionNotificationId", admissionNotificationId),
               new Claim("smsClientId", smsClientId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JwtSettings:Secret").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AdmissionAuth { Token = tokenHandler.WriteToken(token), Expires = tokenDescriptor.Expires.ToString() };
        }
        private async Task SendNotifications(string admissionNotificationId, string receiver, string schoolURL)
        {
            var toEmail = new List<EmailAddress>();
            var frmEmail = new List<EmailAddress>();
            toEmail.Add(new EmailAddress { Address = receiver, Name = receiver });
            frmEmail.Add(new EmailAddress { Address = emailConfiguration.SmtpUsername, Name = emailConfiguration.Sender });
            var host = accessor.HttpContext.Request.Host.ToUriComponent();
            var email = new EmailMessage
            {
                Content = $"Hello, you've Successfully registered. Kindly click the link below to confirm your email address.<br /> {schoolURL}/candidate-admission?admissionNotificationId={admissionNotificationId}",
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
                if(classes != null)
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

                res.IsSuccessful = true;
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

        public async Task<APIResponse<string>> UpdateAdmission(UpdateAdmission request)
        {
            var res = new APIResponse<string>();
            try
            {
                var admissionNotificationId = Guid.Parse(accessor.HttpContext.Items["admissionNotificationId"].ToString());
                var admission = await context.Admissions.Where(m => m.AdmissionId == Guid.Parse(request.AdmissionId) && m.AdmissionNotificationId == admissionNotificationId).FirstOrDefaultAsync();
                if (admission == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AdmissionId doesn't exist";
                    return res;
                }
                var filePath = fileUpload.UploadAdmissionCredentials(request.Credentials);

                string oldCredentials = admission.Credentials.Split("AdmissionCredentials/")[1];

                var photoPath = fileUpload.UploadAdmissionPassport(request.Photo);
                string oldPhoto = admission.Photo.Split("AdmissionPassport/")[1];

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
                admission.ParentName = request.ParentName;
                admission.ParentRelationship = request.ParentRelationship;
                admission.ParentPhoneNumber = request.ParentPhoneNumber;
                admission.ClassId = Guid.Parse(request.ClassId);
                await context.SaveChangesAsync();

                var oldCredentialsPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + "AdmissionCredentials", oldCredentials);
                fileUpload.DeleteFile(oldCredentialsPath);

                var oldPhotoPath = Path.Combine(environment.ContentRootPath, "wwwroot/" + "AdmissionPassport", oldPhoto);
                fileUpload.DeleteFile(oldPhotoPath);

                res.Result = admission.AdmissionId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
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

        public async Task<APIResponse<SelectAdmissionSettings>> GetSettings()
        {
            var res = new APIResponse<SelectAdmissionSettings>();
            try
            {
                var classes = context.AdmissionSettings?.Where(d => d.ClientId == smsClientId && d.Deleted != true)?.FirstOrDefault()?.Classes?.Split(',').ToList();
                var result = await context.AdmissionSettings
                    .Where(d => d.ClientId == smsClientId && d.Deleted != true && d.AdmissionStatus == true)
                    .Select(db => new SelectAdmissionSettings(db, context.ClassLookUp.Where(x => classes.Contains(x.ClassLookupId.ToString())).ToList()))
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = "No admission in progress.";
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
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
