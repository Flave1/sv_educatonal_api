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
        public bool PromoteByPassmark { get; set; }
        public bool PromoteAll { get; set; }
        public bool ShowPositionOnResult { get; set; }
        public bool CumulativeResult { get; set; }
        public bool ShowNewsletter { get; set; }
        public bool BatchPrinting { get; set; }
        public string PrincipalStample { get; set; }
        public string Template { get; set; }
    }
}
