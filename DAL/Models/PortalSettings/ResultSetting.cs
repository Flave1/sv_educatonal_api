using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.PortalSettings
{
    public class ResultSetting: CommonEntity
    {
        [Key]
        public Guid ResultSettingId { get; set; }
        public bool Promote_by_passmark { get; set; }
        public bool Promote_all { get; set; }
        public bool Show_position_on_result { get; set; }
        public bool Cumulative_result { get; set; }
        public bool Show_newsletter { get; set; }
        public bool Batch_printing { get; set; }
    }
}
