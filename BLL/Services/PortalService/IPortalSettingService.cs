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
        Task<APIResponse<PostResultSetting>> CreateUpdateResultSettingsAsync(PostResultSetting contract);
        Task<APIResponse<PostSchoolSetting>> CreateUpdateSchollSettingsAsync(PostSchoolSetting contract);
        Task<APIResponse<PostNotificationSetting>> CreateUpdateNotificationSettingsAsync(PostNotificationSetting contract);
        Task<APIResponse<NotificationSettingContract>> GetNotificationSettingsAsync();
        Task<APIResponse<SchoolSettingContract>> GetSchollSettingsAsync();
        Task<APIResponse<ResultSettingContract>> GetResultSettingsAsync();
    }
}
