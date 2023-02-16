using BLL;
using SMP.Contracts.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PortalService
{
    public interface IPortalSettingService
    {
        Task<APIResponse<PostResultSetting>> CreateUpdateResultSettingsAsync(PostResultSetting request);
        Task<APIResponse<PostSchoolSetting>> CreateUpdateSchollSettingsAsync(PostSchoolSetting request);
        Task<APIResponse<PostNotificationSetting>> CreateUpdateNotificationSettingsAsync(PostNotificationSetting request);
        Task<APIResponse<PostNotificationSetting>> GetNotificationSettingsAsync();
        Task<APIResponse<SchoolSettingContract>> GetSchollSettingsAsync();
        Task<APIResponse<ResultSettingContract>> GetResultSettingsAsync();
        Task<APIResponse<UpdateResultSetting>> UpdateResultSettingTemplateAsync(UpdateResultSetting request);
        Task<APIResponse<AppLayoutSettings>> UpdateAppLayoutSettingsAsync(AppLayoutSettings request);
        Task<APIResponse<AppLayoutSettings>> GetAppLayoutSettingsAsync(string url);
        void CreateAppLayoutSettingsAsync(string clientId, string schoolUrl);
        Task<APIResponse<CreateRegNoSetting>> CreateUpdateRegNoSettingsAsync(CreateRegNoSetting request);
        Task<APIResponse<RegNoSetting>> GetRegNoSettingsAsync();
        Task CreateSchoolSettingsAsync(SMSSMPAccountSetting request, string email);
    }
}
