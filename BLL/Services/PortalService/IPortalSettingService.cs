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
        Task<APIResponse<NotificationSettingContract>> GetNotificationSettingsAsync();
        Task<APIResponse<SchoolSettingContract>> GetSchollSettingsAsync();
        Task<APIResponse<ResultSettingContract>> GetResultSettingsAsync();
        Task<APIResponse<PostResultSetting>> UpdateTemplateAsync(string template);
    }
}
