using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PortalSettings
{
    public class NotificationSetting : CommonEntity
    {
        [Key]
        public Guid NotificationSettingId { get; set; }
        public string RecoverPassword { get; set; }
        public string Announcement { get; set; }
        public string Assessment { get; set; }
        public string Permission { get; set; }
        public string Session { get; set; }
        public string ClassManagement { get; set; }
        public string Staff { get; set; }
        public string Enrollment { get; set; }
        public string PublishResult { get; set; }
        public bool ShouldSendToParentsOnResultPublish { get; set; }
    }
}
