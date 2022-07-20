using SMP.DAL.Models.PortalSettings;
using System;

namespace SMP.Contracts.PortalSettings
{

    public class PostNotificationSetting
    {
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyBySms { get; set; }
    }
    
    public class NotificationSettingContract
    {
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyBySms { get; set; }
        public NotificationSettingContract()
        {

        }
        public NotificationSettingContract(NotificationSetting notification)
        {
            NotificationSettingId = notification.NotificationSettingId;
            NotifyByEmail = notification.NotifyByEmail;
            NotifyBySms = notification.NotifyBySms;
        }
    }
}
