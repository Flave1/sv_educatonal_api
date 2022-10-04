using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
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

        public PortalSettingService(DataContext context, IFileUploadService upload)
        {
            this.context = context;
            this.upload = upload;
        }
        async Task<APIResponse<PostSchoolSetting>> IPortalSettingService.CreateUpdateSchollSettingsAsync(PostSchoolSetting request)
        {
            var res = new APIResponse<PostSchoolSetting>();
            try
            {
                var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync();

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
                var setting = await context.ResultSetting.FirstOrDefaultAsync();
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
            var result = await context.ResultSetting.FirstOrDefaultAsync();
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
            var setting = await context.NotificationSetting.FirstOrDefaultAsync();

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
            res.Result = await context.SchoolSettings.Select(f => new SchoolSettingContract(f)).FirstOrDefaultAsync() ?? new SchoolSettingContract();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ResultSettingContract>> IPortalSettingService.GetResultSettingsAsync()
        {
            var res = new APIResponse<ResultSettingContract>();
            res.Result = await context.ResultSetting.Select(f=> new ResultSettingContract(f)).FirstOrDefaultAsync() ?? new ResultSettingContract();
            res.IsSuccessful = true;
            return res;
        } 
        async Task<APIResponse<PostNotificationSetting>>IPortalSettingService.GetNotificationSettingsAsync()
        {
            var res = new APIResponse<PostNotificationSetting>();
            res.Result = await context.NotificationSetting.Select(f => new PostNotificationSetting(f)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        }
    }
}
