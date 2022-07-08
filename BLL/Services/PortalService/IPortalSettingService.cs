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
        Task<APIResponse<PostResultSetting>> CreateResultSettingsAsync(PostResultSetting contract);
        Task<APIResponse<PostSchoolSetting>> CreateSchollSettingsAsync(PostSchoolSetting contract);
        Task<APIResponse<PostNotificationSetting>> CreateNotificationSettingsAsync(PostNotificationSetting contract);
        Task<APIResponse<List<NotificationSettingContract>>> GetNotificationSettingsAsync(Guid notificationSettingId );
        Task<APIResponse<List<SchoolSettingContract>>> GetSchollSettingsAsync(Guid schoolSettingId);
        Task<APIResponse<List<ResultSettingContract>>> GetResultSettingsAsync(Guid resultSettingId);
    }
}
