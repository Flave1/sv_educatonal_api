using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PortalSettings
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }
        public string NotificationEmailLink { get; set; }
        public string NotificationPageLink { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Senders { get; set; }
        public string Receivers { get; set; }
        public bool IsSent { get; set; }
        public bool IsRead { get; set; }
        public string Svg { get; set; }
        public string NotificationSourceId { get; set; }
        public Guid? NotificationSettingId { get; set; }
        [ForeignKey("NotificationSettingId")]
        public virtual NotificationSetting NotificationSetting { get; set; }
    }
}
