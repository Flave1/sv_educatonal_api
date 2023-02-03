using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMP.BLL.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.Contracts.PortalSettings;
using SMP.DAL.Models.PortalSettings;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PortalService
{
    public class PortalSettingService : IPortalSettingService
    {
        private readonly DataContext context;
        private readonly IFileUploadService upload;
        private readonly string smsClientId;
        public PortalSettingService(DataContext context, IFileUploadService upload, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.upload = upload;
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
                        SchoolName = request.SchoolName,
                        SchoolAddress = request.SchoolAddress,
                        SchoolAbbreviation = request.SchoolAbbreviation,
                        PhoneNo1 = request.PhoneNo1,
                        PhoneNo2 = request.PhoneNo2,
                        SchoolType = request.SchoolType,
                        Country = request.Country,
                        State = request.State,
                        Photo = filePath,
                        Email = request.Email

                    };
                    await context.SchoolSettings.AddAsync(schoolSetting);

                }
                else
                {
                    var filePath = upload.UpdateSchoolLogo(request.Photo, request.Filepath);
                    schoolSetting.SchoolName = request.SchoolName;
                    schoolSetting.SchoolAddress = request.SchoolAddress;
                    schoolSetting.SchoolAbbreviation = request.SchoolAbbreviation;
                    schoolSetting.PhoneNo1 = request.PhoneNo1;
                    schoolSetting.PhoneNo2 = request.PhoneNo2;
                    schoolSetting.SchoolType = request.SchoolType;
                    schoolSetting.Country = request.Country;
                    schoolSetting.State = request.State;
                    schoolSetting.Photo = filePath;
                    schoolSetting.Email = request.Email;
                }
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Saved;
                res.Result = request;
                res.IsSuccessful = true;
                return res;

            }
            catch (System.ArgumentException ex)
            {
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
        }

        async Task<APIResponse<PostResultSetting>> IPortalSettingService.CreateUpdateResultSettingsAsync(PostResultSetting request)
        {
            var res = new APIResponse<PostResultSetting>();
            try
            {
                var setting = await context.ResultSetting.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);
                if (setting == null)
                {
                    var filePath = upload.UploadPrincipalStamp(request.PrincipalStamp);
                    setting = new ResultSetting()
                    {
                        PromoteAll = request.PromoteAll,
                        ShowPositionOnResult = request.ShowPositionOnResult,
                        ShowNewsletter = request.ShowNewsletter,
                        CumulativeResult = request.CumulativeResult,
                        BatchPrinting = request.BatchPrinting,
                        PrincipalStample = filePath
                    };
                    await context.ResultSetting.AddAsync(setting);
                }
                else
                {
                    var filePath = upload.UpdatePrincipalStamp(request.PrincipalStamp, request.Filepath);
                    setting.PromoteAll = request.PromoteAll;
                    setting.ShowPositionOnResult = request.ShowPositionOnResult;
                    setting.ShowNewsletter = request.ShowNewsletter;
                    setting.CumulativeResult = request.CumulativeResult;
                    setting.BatchPrinting = request.BatchPrinting;
                    setting.PrincipalStample = filePath;
                }
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = Messages.Saved;
                res.Result = request;
                res.IsSuccessful = true;
                return res;

            }
            catch (System.ArgumentException ex)
            { 
                res.Message.FriendlyMessage = ex.Message;
                return res;
            }
             
        } 
        async Task<APIResponse<UpdateResultSetting>> IPortalSettingService.UpdateResultSettingTemplateAsync(UpdateResultSetting request)
        {
            var res = new APIResponse<UpdateResultSetting>();
            var result = await context.ResultSetting.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);
            if (result == null)
            { 
                res.Message.FriendlyMessage = "Result Settings Not Found";

                return res;
            }
            else
            {
                result.SelectedTemplate = request.SelectedTemplate;
                await context.SaveChangesAsync();
                res.Message.FriendlyMessage = "Updated Successfully";
                res.IsSuccessful = true;
                res.Result = request;

                return res;
            }
        }
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateUpdateNotificationSettingsAsync(PostNotificationSetting request)
        {
            var res = new APIResponse<PostNotificationSetting>();
            var setting = await context.NotificationSetting.FirstOrDefaultAsync(x=>x.ClientId == smsClientId);

            if (setting is not null)
            {
                setting.Announcement = request.Announcement.media +"/"+ request.Announcement.send;
                setting.Assessment = request.Assessment.media + "/" + request.Assessment.send;
                setting.ClassManagement = request.ClassManagement.media + "/" + request.ClassManagement.send;
                setting.Enrollment = request.Enrollment.media + "/" + request.Enrollment.send;
                setting.Permission = request.Permission.media + "/" + request.Permission.send;
                setting.PublishResult = request.PublishResult.media + "/" + request.PublishResult.send;
                setting.RecoverPassword = request.RecoverPassword.media + "/" + request.RecoverPassword.send;
                setting.Session = request.Session.media + "/" + request.Session.send;
                setting.ShouldSendToParentsOnResultPublish = request.PublishResult.ShouldSendToParentsOnResultPublish;
                setting.Staff = request.Staff.media + "/" + request.Staff.send;
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
            var result = await context.ResultSetting.Where(x=>x.ClientId == smsClientId).Select(f=> new ResultSettingContract(f)).FirstOrDefaultAsync() ?? new ResultSettingContract();
            if (result != null)
            {
                var session = context.Session.FirstOrDefault(x => x.IsActive && x.ClientId == smsClientId);
                if(session is not null)
                {
                    var user = context.Teacher.Where(x => x.ClientId == smsClientId && x.TeacherId == session.HeadTeacherId).Include(c => c.User).Select(x => x.User).FirstOrDefault();
                    result.Headteacher = user?.FirstName + " " + user?.LastName;
                }
               
            }
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        } 
        async Task<APIResponse<PostNotificationSetting>>IPortalSettingService.GetNotificationSettingsAsync()
        {
            var res = new APIResponse<PostNotificationSetting>();
            res.Result = await context.NotificationSetting.Where(x => x.ClientId == smsClientId).Select(f => new PostNotificationSetting(f)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<AppLayoutSettings>> IPortalSettingService.GetAppLayoutSettingsAsync(string url)
        {
            var res = new APIResponse<AppLayoutSettings>();
            res.Result = new AppLayoutSettings();
            var setting = await context.AppLayoutSetting.FirstOrDefaultAsync(x => x.schoolUrl.ToLower() == url.TrimEnd('/').ToLower());

            if (setting is not null)
            {
                res.Result.scheme = setting.scheme;
                res.Result.schemeDir = setting.schemeDir;
                res.Result.colorprimary = setting.colorprimary;
                res.Result.navbarstyle = setting.navbarstyle;
                res.Result.sidebarcolor = setting.sidebarcolor;
                res.Result.colorcustomizer = setting.colorcustomizer;
                res.Result.colorinfo = setting.colorinfo;
                res.Result.loginTemplate = setting.loginTemplate;
                res.Result.sidebarActiveStyle = setting.sidebarActiveStyle;
                res.Result.schoolUrl = setting.schoolUrl;
                res.Result.sidebarType = JsonConvert.DeserializeObject<SidebarType>(setting.sidebarType);
            }

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<AppLayoutSettings>> IPortalSettingService.UpdateAppLayoutSettingsAsync(AppLayoutSettings request)
        {
            var res = new APIResponse<AppLayoutSettings>();
            var setting = await context.AppLayoutSetting.FirstOrDefaultAsync(x => x.ClientId == smsClientId);

            if (setting is not null)
            {
                setting.scheme = request.scheme;
                setting.schemeDir = request.schemeDir;
                setting.colorprimary = request.colorprimary;
                setting.navbarstyle = request.navbarstyle;
                setting.sidebarcolor = request.sidebarcolor;
                setting.colorcustomizer = request.colorcustomizer;
                setting.colorinfo = request.colorinfo;
                setting.sidebarType = JsonConvert.SerializeObject(request.sidebarType);
                setting.loginTemplate = request.loginTemplate;
                setting.sidebarActiveStyle = request.sidebarActiveStyle;
                setting.schoolUrl = request.schoolUrl;
                await context.SaveChangesAsync();
            }
            else
            {
                setting = new AppLayoutSetting();
                setting.scheme = request.scheme;
                setting.schemeDir = request.schemeDir;
                setting.colorprimary = request.colorprimary;
                setting.navbarstyle = request.navbarstyle;
                setting.sidebarcolor = request.sidebarcolor;
                setting.colorcustomizer = request.colorcustomizer;
                setting.colorinfo = request.colorinfo;
                setting.sidebarType= JsonConvert.SerializeObject(request.sidebarType);
                setting.loginTemplate = request.loginTemplate;
                setting.schoolUrl = request.schoolUrl;
                setting.sidebarActiveStyle = request.sidebarActiveStyle;
                context.AppLayoutSetting.Add(setting);
                context.SaveChanges();
            }

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            res.IsSuccessful = true;
            return res;
        }
    }
}
