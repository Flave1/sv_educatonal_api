using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PortalSettings
{
    public class NotificationSetting : CommonEntity
    {
        [Key]
        public Guid NotificationSettingId { get; set; }
        public bool NotifyByEmail { get; set; }
    }
}
