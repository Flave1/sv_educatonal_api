using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PortalSettings
{
    public class NotificationSetting : CommonEntity
    {
        [Key]
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyBySms { get; set; }
    }
}
