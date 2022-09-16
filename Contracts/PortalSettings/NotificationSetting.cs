using SMP.DAL.Models.PortalSettings;
using System;

namespace SMP.Contracts.PortalSettings
{

    public class PostNotificationSetting
    {
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
    
    public class NotificationSettingContract
    {
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
        public NotificationSettingContract() { }
        public NotificationSettingContract(NotificationSetting notification)
        {
            Announcement = notification.Announcement;
            Assessment = notification.Assessment;
            ClassManagement = notification.ClassManagement;
            Enrollment = notification.Enrollment;
            Permission = notification.Permission;
            PublishResult = notification.PublishResult;
            RecoverPassword = notification.RecoverPassword;
            Session = notification.Session;
            ShouldSendToParentsOnResultPublish = notification.ShouldSendToParentsOnResultPublish;
            Staff = notification.Staff;
        }
    }
}
