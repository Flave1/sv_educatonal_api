using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.FileUpload;
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
            var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync();
          
            if (schoolSetting == null)
            {
                var filePath = upload.UploadProfileImage(request.Photo);
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
                    Photo = filePath

                };
                await context.SchoolSettings.AddAsync(schoolSetting);

            }
            else
            {
                var filePath = upload.UpdateProfileImage(request.Photo, request.Filepath);
                schoolSetting.SchoolName = request.SchoolName;
                schoolSetting.SchoolAddress = request.SchoolAddress;
                schoolSetting.SchoolAbbreviation = request.SchoolAbbreviation;
                schoolSetting.PhoneNo1 = request.PhoneNo1;
                schoolSetting.PhoneNo2 = request.PhoneNo2;
                schoolSetting.SchoolType = request.SchoolType;
                schoolSetting.Country = request.Country;
                schoolSetting.State = request.State;
                schoolSetting.Photo = filePath;
            }
            await context.SaveChangesAsync();
            res.Message.FriendlyMessage = Messages.Saved;
            res.Result = request;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PostResultSetting>> IPortalSettingService.CreateUpdateResultSettingsAsync(PostResultSetting request)
        {
            var res = new APIResponse<PostResultSetting>();
            var setting = await context.ResultSetting.FirstOrDefaultAsync();
            if (setting == null)
            {
                var filePath = upload.UploadProfileImage(request.PrincipalStamp);
                setting = new ResultSetting()
                {
                    PromoteByPassmark = request.PromoteByPassmark,
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
                var filePath = upload.UpdateProfileImage(request.PrincipalStamp, request.Filepath);
                setting.PromoteByPassmark = request.PromoteByPassmark;
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
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateUpdateNotificationSettingsAsync(PostNotificationSetting request)
        async Task<APIResponse<PostResultSetting>> IPortalSettingService.UpdateTemplateAsync(string template)
        {
            var res = new APIResponse<PostResultSetting>();
            var result = await context.ResultSetting.FirstOrDefaultAsync(x => x.Deleted == false);
            if (result == null)
            { 
                res.Message.FriendlyMessage = "Result Settings Not Found";
            }
            else
            {
                result.SelectedTemplate = template;
                await context.SaveChangesAsync();
            }
            res.Message.FriendlyMessage = "Updated Successfully";
            res.Result = new PostResultSetting { ResultSettingId = result.ResultSettingId, BatchPrinting = result.BatchPrinting, CumulativeResult = result.CumulativeResult, SelectedTemplate = result.SelectedTemplate, PromoteAll = result.PromoteAll, PromoteByPassmark = result.PromoteByPassmark, ShowNewsletter = result.ShowNewsletter, ShowPositionOnResult = result.ShowPositionOnResult };
            return res;
        }
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateUpdateNotificationSettingsAsync(PostNotificationSetting contract)
        {
            var res = new APIResponse<PostNotificationSetting>();
            var setting = await context.NotificationSetting.FirstOrDefaultAsync();

            if (setting == null)
            { 
                setting = new NotificationSetting
                {
                    NotifyByEmail = request.NotifyByEmail,
                };
                await context.NotificationSetting.AddAsync(setting);
            }
            else
            {
                setting.NotifyByEmail = request.NotifyByEmail;
            }
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = request;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<SchoolSettingContract>> IPortalSettingService.GetSchollSettingsAsync()
        async Task<APIResponse<SchoolSettingContract>> IPortalSettingService.GetSchollSettingsAsync()
        {
            var res = new APIResponse<SchoolSettingContract>();
            res.Result = await context.SchoolSettings.Select(f => new SchoolSettingContract(f)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<ResultSettingContract>> IPortalSettingService.GetResultSettingsAsync()
        {
            var res = new APIResponse<ResultSettingContract>();
            res.Result = await context.ResultSetting.Select(f=> new ResultSettingContract(f)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        } 
        async Task<APIResponse<NotificationSettingContract>>IPortalSettingService.GetNotificationSettingsAsync()
        {
            var res = new APIResponse<NotificationSettingContract>();
            res.Result =  await  context.NotificationSetting.Select(f=> new NotificationSettingContract(f)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            return res;
        }
    }
}
