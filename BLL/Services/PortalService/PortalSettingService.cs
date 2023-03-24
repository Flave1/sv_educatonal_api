using BLL;
using BLL.LoggerService;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.Contracts.PortalSettings;
using SMP.DAL.Migrations;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PortalService
{
    public class PortalSettingService : IPortalSettingService
    {
        private readonly DataContext context;
        private readonly IFileUploadService upload;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        public PortalSettingService(DataContext context, IFileUploadService upload, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.upload = upload;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }
        async Task<APIResponse<PostSchoolSetting>> IPortalSettingService.CreateUpdateSchollSettingsAsync(PostSchoolSetting request)
        {
            var res = new APIResponse<PostSchoolSetting>();
            try
            {
                var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);

                if (schoolSetting == null)
                {
                    var filePath = upload.UploadSchoolLogo(request.Photo);
                    schoolSetting = new SchoolSetting()
                    {
                        SCHOOLSETTINGS_SchoolName = request.SchoolName,
                        SCHOOLSETTINGS_SchoolAddress = request.SchoolAddress,
                        SCHOOLSETTINGS_SchoolAbbreviation = request.SchoolAbbreviation,
                        SCHOOLSETTINGS_PhoneNo1 = request.PhoneNo1,
                        SCHOOLSETTINGS_PhoneNo2 = request.PhoneNo2,
                        SCHOOLSETTINGS_SchoolType = request.SchoolType,
                        SCHOOLSETTINGS_Country = request.Country,
                        SCHOOLSETTINGS_State = request.State,
                        SCHOOLSETTINGS_Photo = filePath,
                        SCHOOLSETTINGS_Email = request.Email

                    };
                    await context.SchoolSettings.AddAsync(schoolSetting);

                }
                else
                {
                    var filePath = upload.UpdateSchoolLogo(request.Photo, request.Filepath);
                    schoolSetting.SCHOOLSETTINGS_SchoolName = request.SchoolName;
                    schoolSetting.SCHOOLSETTINGS_SchoolAddress = request.SchoolAddress;
                    schoolSetting.SCHOOLSETTINGS_SchoolAbbreviation = request.SchoolAbbreviation;
                    schoolSetting.SCHOOLSETTINGS_PhoneNo1 = request.PhoneNo1;
                    schoolSetting.SCHOOLSETTINGS_PhoneNo2 = request.PhoneNo2;
                    schoolSetting.SCHOOLSETTINGS_SchoolType = request.SchoolType;
                    schoolSetting.SCHOOLSETTINGS_Country = request.Country;
                    schoolSetting.SCHOOLSETTINGS_State = request.State;
                    schoolSetting.SCHOOLSETTINGS_Photo = filePath;
                    schoolSetting.SCHOOLSETTINGS_Email = request.Email;
                }
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Saved;
                res.Result = request;
                res.IsSuccessful = true;
                return res;

            }
            catch (System.ArgumentException ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }

        async Task<APIResponse<PostResultSetting>> IPortalSettingService.CreateUpdateResultSettingsAsync(PostResultSetting request)
        {
            var res = new APIResponse<PostResultSetting>();
            try
            {
                var setting = await context.SchoolSettings.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);
                if (setting == null)
                {
                    var filePath = upload.UploadPrincipalStamp(request.PrincipalStamp);
                    setting = new SchoolSetting()
                    {
                        RESULTSETTINGS_PromoteAll = request.PromoteAll,
                        RESULTSETTINGS_ShowPositionOnResult = request.ShowPositionOnResult,
                        RESULTSETTINGS_ShowNewsletter = request.ShowNewsletter,
                        RESULTSETTINGS_CumulativeResult = request.CumulativeResult,
                        RESULTSETTINGS_BatchPrinting = request.BatchPrinting,
                        RESULTSETTINGS_PrincipalStample = filePath
                    };
                    await context.SchoolSettings.AddAsync(setting);
                }
                else
                {
                    var filePath = upload.UpdatePrincipalStamp(request.PrincipalStamp, request.Filepath);
                    setting.RESULTSETTINGS_PromoteAll = request.PromoteAll;
                    setting.RESULTSETTINGS_ShowPositionOnResult = request.ShowPositionOnResult;
                    setting.RESULTSETTINGS_ShowNewsletter = request.ShowNewsletter;
                    setting.RESULTSETTINGS_CumulativeResult = request.CumulativeResult;
                    setting.RESULTSETTINGS_BatchPrinting = request.BatchPrinting;
                    setting.RESULTSETTINGS_PrincipalStample = filePath;
                }
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Saved;
                res.Result = request;
                res.IsSuccessful = true;
                return res;

            }
            catch (System.ArgumentException ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
             
        } 
        async Task<APIResponse<UpdateResultSetting>> IPortalSettingService.UpdateResultSettingTemplateAsync(UpdateResultSetting request)
        {
            var res = new APIResponse<UpdateResultSetting>();
            var result = await context.SchoolSettings.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);
            if (result == null)
            {
                var setting = new SchoolSetting()
                {
                    RESULTSETTINGS_PromoteAll = true,
                    RESULTSETTINGS_ShowPositionOnResult = false,
                    RESULTSETTINGS_ShowNewsletter = false,
                    RESULTSETTINGS_CumulativeResult = true,
                    RESULTSETTINGS_BatchPrinting = true,
                    RESULTSETTINGS_PrincipalStample = "",
                    RESULTSETTINGS_SelectedTemplate = request.SelectedTemplate
                };
                context.Add(setting);
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = "Template selected successfully";
                res.IsSuccessful = true;
                return res;
            }
            else
            {
                result.RESULTSETTINGS_SelectedTemplate = request.SelectedTemplate;
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = "Template selected Successfully";
                res.IsSuccessful = true;
                res.Result = request;

                return res;
            }
        }
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateUpdateNotificationSettingsAsync(PostNotificationSetting request)
        {
            var res = new APIResponse<PostNotificationSetting>();
            var setting = await context.SchoolSettings.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);

            if (setting is not null)
            {
                setting.NOTIFICATIONSETTINGS_Announcement = request.Announcement.media +"/"+ request.Announcement.send;
                setting.NOTIFICATIONSETTINGS_Assessment = request.Assessment.media + "/" + request.Assessment.send;
                setting.NOTIFICATIONSETTINGS_ClassManagement = request.ClassManagement.media + "/" + request.ClassManagement.send;
                setting.NOTIFICATIONSETTINGS_Enrollment = request.Enrollment.media + "/" + request.Enrollment.send;
                setting.NOTIFICATIONSETTINGS_Permission = request.Permission.media + "/" + request.Permission.send;
                setting.NOTIFICATIONSETTINGS_PublishResult = request.PublishResult.media + "/" + request.PublishResult.send;
                setting.NOTIFICATIONSETTINGS_RecoverPassword = request.RecoverPassword.media + "/" + request.RecoverPassword.send;
                setting.NOTIFICATIONSETTINGS_Session = request.Session.media + "/" + request.Session.send;
                setting.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = request.PublishResult.ShouldSendToParentsOnResultPublish;
                setting.NOTIFICATIONSETTINGS_Staff = request.Staff.media + "/" + request.Staff.send;
                await context.SaveChangesAsync();
            }

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            res.IsSuccessful = true;
            return res;
        }
         
        async Task<APIResponse<SchoolSettingContract>> IPortalSettingService.GetSchollSettingsAsync()
        {
            var res = new APIResponse<SchoolSettingContract>();
            res.Result = await context.SchoolSettings.Where(x=>x.ClientId == smsClientId).Select(f => new SchoolSettingContract(f)).FirstOrDefaultAsync() ?? new SchoolSettingContract();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ResultSettingContract>> IPortalSettingService.GetResultSettingsAsync()
        {
            var res = new APIResponse<ResultSettingContract>();
            var result = await context.SchoolSettings.Where(x=>x.ClientId == smsClientId).Select(f=> new ResultSettingContract(f)).FirstOrDefaultAsync() ?? new ResultSettingContract();
            if (result != null)
            {
                var session = context.Session.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId);
                if(session is not null)
                {
                    var teacher = context.Teacher.Where(x => x.ClientId == smsClientId && x.TeacherId == session.HeadTeacherId).FirstOrDefault();
                    result.Headteacher = teacher?.FirstName + " " + teacher?.LastName;
                }
               
            }
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        } 
        async Task<APIResponse<PostNotificationSetting>>IPortalSettingService.GetNotificationSettingsAsync()
        {
            var res = new APIResponse<PostNotificationSetting>();
            res.Result = await context.SchoolSettings.Where(x => x.ClientId == smsClientId).Select(f => new PostNotificationSetting(f)).FirstOrDefaultAsync() ?? new PostNotificationSetting();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<AppLayoutSettings>> IPortalSettingService.GetAppLayoutSettingsAsync(string url)
        {
            //await loggerService.Error("settings", url, "settings", "settings");
            var res = new APIResponse<AppLayoutSettings>();
            res.Result = new AppLayoutSettings();
            var setting = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl.ToLower() == url.ToLower());

            if (setting is not null)
            {
                res.Result.scheme = setting.APPLAYOUTSETTINGS_Scheme;
                res.Result.schemeDir = setting.APPLAYOUTSETTINGS_SchemeDir;
                res.Result.colorprimary = setting.APPLAYOUTSETTINGS_Colorprimary;
                res.Result.navbarstyle = setting.APPLAYOUTSETTINGS_Navbarstyle;
                res.Result.sidebarcolor = setting.APPLAYOUTSETTINGS_Sidebarcolor;
                res.Result.colorcustomizer = setting.APPLAYOUTSETTINGS_Colorcustomizer;
                res.Result.colorinfo = setting.APPLAYOUTSETTINGS_Colorinfo;
                res.Result.loginTemplate = setting.APPLAYOUTSETTINGS_loginTemplate;
                res.Result.sidebarActiveStyle = setting.APPLAYOUTSETTINGS_SidebarActiveStyle;
                res.Result.schoolUrl = setting.APPLAYOUTSETTINGS_SchoolUrl;
                res.Result.schoolName = setting.SCHOOLSETTINGS_SchoolName;
                res.Result.schoolLogo = setting.SCHOOLSETTINGS_Photo;
                if(setting.APPLAYOUTSETTINGS_SidebarType is not null)
                    res.Result.sidebarType = JsonConvert.DeserializeObject<SidebarType>(setting.APPLAYOUTSETTINGS_SidebarType);
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<AppLayoutSettings>> IPortalSettingService.UpdateAppLayoutSettingsAsync(AppLayoutSettings request)
        {
            var res = new APIResponse<AppLayoutSettings>();
            var setting = await context.SchoolSettings.FirstOrDefaultAsync(x => request.schoolUrl == x.APPLAYOUTSETTINGS_SchoolUrl);

            if (setting is not null)
            {
                setting.APPLAYOUTSETTINGS_Scheme = request.scheme;
                setting.APPLAYOUTSETTINGS_SchemeDir = request.schemeDir;
                setting.APPLAYOUTSETTINGS_Colorprimary = request.colorprimary;
                setting.APPLAYOUTSETTINGS_Navbarstyle = request.navbarstyle;
                setting.APPLAYOUTSETTINGS_Sidebarcolor = request.sidebarcolor;
                setting.APPLAYOUTSETTINGS_Colorcustomizer = request.colorcustomizer;
                setting.APPLAYOUTSETTINGS_Colorinfo = request.colorinfo;
                setting.APPLAYOUTSETTINGS_SidebarType = JsonConvert.SerializeObject(request.sidebarType);
                setting.APPLAYOUTSETTINGS_loginTemplate = request.loginTemplate;
                setting.APPLAYOUTSETTINGS_SidebarActiveStyle = request.sidebarActiveStyle;
                setting.APPLAYOUTSETTINGS_SchoolUrl = request.schoolUrl;
                setting.ClientId = smsClientId;
                await context.SaveChangesAsync();
            }
            

            res.Message.FriendlyMessage = "Updated Successfully";
            res.Result = request;
            res.IsSuccessful = true;
            return res;
        }

        void IPortalSettingService.CreateSchoolSettingsAsync(string clientId, string schoolUrl)
        {
            try
            {
                var isNew = false;
                var setting = context.SchoolSettings.Where(x => x.ClientId == clientId).FirstOrDefault();
                if(setting == null)
                {
                    setting = new SchoolSetting();
                    isNew = true;
                }

                //APPLAYOUTSETTINGS
                setting.APPLAYOUTSETTINGS_Scheme = "light";
                setting.APPLAYOUTSETTINGS_SchemeDir = "ltr";
                setting.APPLAYOUTSETTINGS_Colorprimary = "#3a57e8";
                setting.APPLAYOUTSETTINGS_Navbarstyle = "sticky";
                setting.APPLAYOUTSETTINGS_Sidebarcolor = "white";
                setting.APPLAYOUTSETTINGS_Colorcustomizer = "default";
                setting.APPLAYOUTSETTINGS_Colorinfo = "#4bc7d2";
                setting.APPLAYOUTSETTINGS_SidebarType = JsonConvert.SerializeObject(new SidebarType());
                setting.APPLAYOUTSETTINGS_loginTemplate = "default-login-template";
                setting.APPLAYOUTSETTINGS_SchoolUrl = schoolUrl;
                setting.APPLAYOUTSETTINGS_SidebarActiveStyle = "roundedAllSide";
                setting.ClientId = clientId;

                //NOTIFICATIONSETTINGS
                setting.NOTIFICATIONSETTINGS_Announcement = "email/false";
                setting.NOTIFICATIONSETTINGS_Assessment = "email/false";
                setting.NOTIFICATIONSETTINGS_ClassManagement = "email/false";
                setting.NOTIFICATIONSETTINGS_Enrollment = "email/false";
                setting.NOTIFICATIONSETTINGS_Permission = "email/false";
                setting.NOTIFICATIONSETTINGS_PublishResult = "email/false";
                setting.NOTIFICATIONSETTINGS_RecoverPassword = "email/false";
                setting.NOTIFICATIONSETTINGS_Session = "email/false";
                setting.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = false;
                setting.NOTIFICATIONSETTINGS_Staff = "email/false";

                if (isNew) context.SchoolSettings.Add(setting);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<APIResponse<CreateRegNoSetting>> CreateUpdateRegNoSettingsAsync(CreateRegNoSetting request)
        {
            var res = new APIResponse<CreateRegNoSetting>();
            try
            {
                var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync(x => x.ClientId == smsClientId);

                if (schoolSetting == null)
                {
                    string studentRegNoFormat = GenerateStudentRegNoFormat(request);
                    string teacherRegNoFormat = GenerateTeacherRegNoFormat(request);

                    schoolSetting = new SchoolSetting()
                    {
                        SCHOOLSETTINGS_StudentRegNoFormat = studentRegNoFormat,
                        SCHOOLSETTINGS_RegNoPosition = request.RegNoPosition,
                        RESULTSETTINGS_SelectedTemplate = teacherRegNoFormat,
                        SCHOOLSETTINGS_RegNoSeperator = request.RegNoSeperator
                    };
                    await context.SchoolSettings.AddAsync(schoolSetting);

                }
                else
                {
                    string studentRegNoFormat = GenerateStudentRegNoFormat(request);
                    string teacherRegNoFormat = GenerateTeacherRegNoFormat(request);

                    schoolSetting.SCHOOLSETTINGS_StudentRegNoFormat = studentRegNoFormat;
                    schoolSetting.SCHOOLSETTINGS_RegNoPosition = request.RegNoPosition;
                    schoolSetting.SCHOOLSETTINGS_TeacherRegNoFormat = teacherRegNoFormat;
                    schoolSetting.SCHOOLSETTINGS_RegNoSeperator = request.RegNoSeperator;
                }

                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Saved;
                res.Result = request;
                res.IsSuccessful = true;
                return res;

            }
            catch (System.ArgumentException ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }
        public async Task<APIResponse<RegNoSetting>> GetRegNoSettingsAsync()
        {
            var res = new APIResponse<RegNoSetting>();
            try
            {
                res.Result = await context.SchoolSettings.Where(x => x.ClientId == smsClientId).Select(f => new RegNoSetting(f)).FirstOrDefaultAsync() ?? new RegNoSetting();
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        private string GenerateStudentRegNoFormat(CreateRegNoSetting request)
        {
            string studentRegNoFormat = "";
            if (request.RegNoPosition == 1)
                studentRegNoFormat = $"%VALUE%{request.RegNoSeperator}{request.StudentRegNoPrefix}{request.RegNoSeperator}{request.StudentRegNoSufix}";
            if (request.RegNoPosition == 2)
                studentRegNoFormat = $"{request.StudentRegNoPrefix}{request.RegNoSeperator}%VALUE%{request.RegNoSeperator}{request.StudentRegNoSufix}";
            if (request.RegNoPosition == 3)
                studentRegNoFormat = $"{request.StudentRegNoPrefix}{request.RegNoSeperator}{request.StudentRegNoSufix}{request.RegNoSeperator}%VALUE%";

            return studentRegNoFormat;
        }
        private string GenerateTeacherRegNoFormat(CreateRegNoSetting request)
        {
            string teacherRegNoFormat = "";
            if (request.RegNoPosition == 1)
                teacherRegNoFormat = $"%VALUE%{request.RegNoSeperator}{request.TeacherRegNoPrefix}{request.RegNoSeperator}{request.TeacherRegNoSufix}";
            if (request.RegNoPosition == 2)
                teacherRegNoFormat = $"{request.TeacherRegNoPrefix}{request.RegNoSeperator}%VALUE%{request.RegNoSeperator}{request.TeacherRegNoSufix}";
            if (request.RegNoPosition == 3)
                teacherRegNoFormat = $"{request.TeacherRegNoPrefix}{request.RegNoSeperator}{request.TeacherRegNoSufix}{request.RegNoSeperator}%VALUE%";

            return teacherRegNoFormat;
        }

        async Task IPortalSettingService.CreateSchoolSettingsAsync(SMSSMPAccountSetting request, string email)
        {
            try
            {
                var isNew = false;
                var splittedWords = request.SchoolName.Split(' ');
                var schoolSetting = context.SchoolSettings.Where(x => x.ClientId == request.ClientId).FirstOrDefault();

                if(schoolSetting is null)
                {
                    schoolSetting = new SchoolSetting();
                    isNew = true;
                }

                schoolSetting.SCHOOLSETTINGS_SchoolName = request.SchoolName;
                schoolSetting.SCHOOLSETTINGS_SchoolAddress = request.Address;
                for(int i = 0; i < splittedWords.Length; i++)
                    schoolSetting.SCHOOLSETTINGS_SchoolAbbreviation += " " + splittedWords[i].Substring(0, 1).ToUpper();
                schoolSetting.SCHOOLSETTINGS_Country = request.Country;
                schoolSetting.SCHOOLSETTINGS_State = request.State;
                schoolSetting.SCHOOLSETTINGS_Email = email;
                schoolSetting.ClientId = request.ClientId;

                if (isNew) await context.SchoolSettings.AddAsync(schoolSetting);
            }
            catch (ArgumentException ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

    }
}
