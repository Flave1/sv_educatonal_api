using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class PostResultSetting
    {
        public Guid ResultSettingId { get; set; }
        public bool Promote_by_passmark { get; set; }
        public bool Promote_all { get; set; }
        public bool Show_position_on_result { get; set; }
        public bool Cumulative_result { get; set; }
        public bool Show_newsletter { get; set; }
        public bool Batch_printing { get; set; }
    }
    public class ResultSettingContract
    {
        public Guid ResultSettingId { get; set; }
        public bool Promote_by_passmark { get; set; }
        public bool Promote_all { get; set; }
        public bool Show_position_on_result { get; set; }
        public bool Cumulative_result { get; set; }
        public bool Show_newsletter { get; set; }
        public bool Batch_printing { get; set; }
        public ResultSettingContract( ResultSetting db)
        {
            ResultSettingId = db.ResultSettingId;
            Promote_by_passmark = db.Promote_by_passmark;
            Promote_all = db.Promote_all;
            Show_position_on_result = db.Show_position_on_result;
            Cumulative_result = db.Cumulative_result;
            Show_newsletter = db.Show_newsletter;
            Batch_printing = db.Batch_printing;
        }
    }
}
