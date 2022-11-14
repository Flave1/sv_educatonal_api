using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PortalSettings
{
    public class Notification: CommonEntity
    {
        [Key]
        public Guid NotificationId { get; set; }
        public string NotificationEmailLink { get; set; }
        public string NotificationPageLink { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Senders { get; set; }
        public string Receivers { get; set; }
        public string ReceiversEmail { get; set; }
        public string ReadBy { get; set; }
        public bool IsSent { get; set; }
        public bool IsRead { get; set; }
        public string Svg { get; set; }
        public string Type { get; set; }
        public string NotificationSourceId { get; set; }
        public string ToGroup { get; set; }
    }
}
