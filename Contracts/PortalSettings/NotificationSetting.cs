using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{

    public class PostNotificationSetting
    {
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail
        {
            get; set;
        }
    }
    public class NotificationSettingContract
    {
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail { get; set; }
        public NotificationSettingContract()
        {

        }
        public NotificationSettingContract(NotificationSetting notification)
        {
            NotificationSettingId = notification.NotificationSettingId;
            NotifyByEmail = notification.NotifyByEmail;
        }
    }
}
