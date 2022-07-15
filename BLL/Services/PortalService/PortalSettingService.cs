using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.FileUpload;
using SMP.Contracts.PortalSettings;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        async Task<APIResponse<PostSchoolSetting>> IPortalSettingService.CreateUpdateSchollSettingsAsync(PostSchoolSetting contract)
        {
            var res = new APIResponse<PostSchoolSetting>();
            var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync(x => x.SchoolSettingsId == contract.SchoolSettingsId);
            var uploadPhoto = upload.UploadProfileImage(contract.Photo);
            if (schoolSetting == null)
            {

                schoolSetting = new SchoolSetting()
                {
                    SchoolName = contract.SchoolName,
                    SchoolAddress = contract.SchoolAddress,
                    SchoolAbbreviation = contract.SchoolAbbreviation,
                    PhoneNo1 = contract.PhoneNo1,
                    PhoneNo2 = contract.PhoneNo2,
                    SchoolType = contract.SchoolType,
                    Country = contract.Country,
                    State = contract.State,
                    Photo = uploadPhoto

                };
                await context.SchoolSettings.AddAsync(schoolSetting);

            }
            else
            {
                var updatePhoto = upload.UpdateProfileImage(contract.Photo,contract.Filepath);
                schoolSetting.SchoolName = contract.SchoolName;
                schoolSetting.SchoolAddress = contract.SchoolAddress;
                schoolSetting.SchoolAbbreviation = contract.SchoolAbbreviation;
                schoolSetting.PhoneNo1 = contract.PhoneNo1;
                schoolSetting.PhoneNo2 = contract.PhoneNo2;
                schoolSetting.SchoolType = contract.SchoolType;
                schoolSetting.Country = contract.Country;
                schoolSetting.State = contract.State;
                schoolSetting.Photo = updatePhoto;
            }
            await context.SaveChangesAsync();
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = new PostSchoolSetting { SchoolSettingsId = schoolSetting.SchoolSettingsId, SchoolAbbreviation = schoolSetting.SchoolAbbreviation, Country = schoolSetting.Country, PhoneNo1 = schoolSetting.PhoneNo1, PhoneNo2 = schoolSetting.PhoneNo2, SchoolAddress = schoolSetting.SchoolAddress, SchoolName = schoolSetting.SchoolName, SchoolType = schoolSetting.SchoolType, State = schoolSetting.State};
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PostResultSetting>> IPortalSettingService.CreateUpdateResultSettingsAsync(PostResultSetting contract)
        {
            var res = new APIResponse<PostResultSetting>();
            var setting = await context.ResultSetting.FirstOrDefaultAsync(x => x.ResultSettingId == contract.ResultSettingId);
            var uploadPhoto = upload.UploadProfileImage(contract.PrincipalStamp);
            if (setting == null)
            {
                setting = new ResultSetting()
                {
                    PromoteByPassmark = contract.PromoteByPassmark,
                    PromoteAll = contract.PromoteAll,
                    ShowPositionOnResult = contract.ShowPositionOnResult,
                    ShowNewsletter = contract.ShowNewsletter,
                    CumulativeResult = contract.CumulativeResult,
                    BatchPrinting = contract.BatchPrinting,
                    PrincipalStample = uploadPhoto
                };
                await context.ResultSetting.AddAsync(setting);
                 
            }
            else
            {
                var updatePhoto = upload.UpdateProfileImage(contract.PrincipalStamp, contract.Filepath);
                setting.PromoteByPassmark = contract.PromoteByPassmark;
                setting.PromoteAll = contract.PromoteAll;
                setting.ShowPositionOnResult = contract.ShowPositionOnResult;
                setting.ShowNewsletter = contract.ShowNewsletter;
                setting.CumulativeResult = contract.CumulativeResult;
                setting.BatchPrinting = contract.BatchPrinting;
                setting.PrincipalStample = updatePhoto;
            }
            await context.SaveChangesAsync();
            res.Message.FriendlyMessage = Messages.Created;
            res.Result = new PostResultSetting { ResultSettingId = setting.ResultSettingId, BatchPrinting = setting.BatchPrinting, CumulativeResult = setting.CumulativeResult, PromoteAll = setting.PromoteAll, PromoteByPassmark = setting.PromoteByPassmark, ShowNewsletter = setting.ShowNewsletter, ShowPositionOnResult = setting.ShowPositionOnResult};
            res.IsSuccessful = true;
            return res;
             
        }
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateUpdateNotificationSettingsAsync(PostNotificationSetting contract)
        {
            var res = new APIResponse<PostNotificationSetting>();
            var setting = await context.NotificationSetting.FirstOrDefaultAsync(x => x.NotificationSettingId == contract.NotificationSettingId);


            if (setting == null)
            { 
                setting = new NotificationSetting
                {
                    NotifyByEmail = contract.NotifyByEmail,
                };
                await context.NotificationSetting.AddAsync(setting);
            }
            else
            {
                setting.NotifyByEmail = contract.NotifyByEmail;
            }
            await context.SaveChangesAsync();

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = new PostNotificationSetting { NotificationSettingId = setting.NotificationSettingId, NotifyByEmail = setting.NotifyByEmail};
            res.IsSuccessful = true;
            return res;
        }
            async Task<APIResponse<List<SchoolSettingContract>>> IPortalSettingService.GetSchollSettingsAsync(Guid schoolSettingId)
        {
            var res = new APIResponse<List<SchoolSettingContract>>();
            var getSettings = await context.SchoolSettings.Where(x => x.Deleted == false && x.SchoolSettingsId == schoolSettingId).Select(a => new SchoolSettingContract(a)).ToListAsync();
            res.Result = getSettings;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<List<ResultSettingContract>>> IPortalSettingService.GetResultSettingsAsync(Guid resultSettingId)
        {
            var res = new APIResponse<List<ResultSettingContract>>();
            var getSettings = await context.ResultSetting.Where(x => x.Deleted == false && x.ResultSettingId == resultSettingId).Select(a => new ResultSettingContract(a)).ToListAsync();
            res.Result = getSettings;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<List<NotificationSettingContract>>> IPortalSettingService.GetNotificationSettingsAsync(Guid notificationSettingId)
        {
            var res = new APIResponse<List<NotificationSettingContract>>();
            var getSettings = await context.NotificationSetting.Where(x => x.Deleted == false && x.NotificationSettingId == notificationSettingId).Select(a => new NotificationSettingContract(a)).ToListAsync();
            res.Result = getSettings;
            res.IsSuccessful = true;
            return res;
        }
    }
}
