using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.PortalSettings;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.Portal
{
    public class PortalSettingService : IPortalSettingService
    {
        private readonly DataContext context;
        public PortalSettingService()
        {
            this.context = context;
        }
        async Task<APIResponse<PostSchoolSetting>> IPortalSettingService.CreateSchollSettingsAsync(PostSchoolSetting contract)
        {
            var res = new APIResponse<PostSchoolSetting>();
            var schoolSetting = await context.SchoolSettings.FirstOrDefaultAsync(x => x.SchoolSettingsId == contract.SchoolSettingsId);

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

                };
                await context.SchoolSettings.AddAsync(schoolSetting);

            }
            else
            {
                schoolSetting.SchoolName = contract.SchoolName;
                schoolSetting.SchoolAddress = contract.SchoolAddress;
                schoolSetting.SchoolAbbreviation = contract.SchoolAbbreviation;
                schoolSetting.PhoneNo1 = contract.PhoneNo1;
                schoolSetting.PhoneNo2 = contract.PhoneNo2;
                schoolSetting.SchoolType = contract.SchoolType;
                schoolSetting.Country = contract.Country;
                schoolSetting.State = contract.State;
            }

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = contract;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<PostResultSetting>> IPortalSettingService.CreateResultSettingsAsync(PostResultSetting contract)
        {
            var res = new APIResponse<PostResultSetting>();
            var setting = await context.ResultSetting.FirstOrDefaultAsync(x => x.ResultSettingId == contract.ResultSettingId);
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
                };
                await context.ResultSetting.AddAsync(setting);

            }
            else
            {
                setting.PromoteByPassmark = contract.PromoteByPassmark;
                setting.PromoteAll = contract.PromoteAll;
                setting.ShowPositionOnResult = contract.ShowPositionOnResult;
                setting.ShowNewsletter = contract.ShowNewsletter;
                setting.CumulativeResult = contract.CumulativeResult;
                setting.BatchPrinting = contract.BatchPrinting;
            }

            res.Message.FriendlyMessage = Messages.Created;
            res.Result = contract;
            res.IsSuccessful = true;
            return res;
             
        }
        async Task<APIResponse<PostNotificationSetting>> IPortalSettingService.CreateNotificationSettingsAsync(PostNotificationSetting contract)
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
            res.Result = contract;
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
