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
        public bool PromoteByPassmark { get; set; }
        public bool PromoteAll { get; set; }
        public bool ShowPositionOnResult { get; set; }
        public bool CumulativeResult { get; set; }
        public bool ShowNewsletter { get; set; }
        public bool BatchPrinting { get; set; }
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
            Promote_by_passmark = db.PromoteByPassmark;
            Promote_all = db.PromoteAll;
            Show_position_on_result = db.ShowPositionOnResult;
            Cumulative_result = db.CumulativeResult;
            Show_newsletter = db.ShowNewsletter;
            Batch_printing = db.BatchPrinting;
        }
    }
}
