using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class UpdateResultSetting
    {
        public string SelectedTemplate { get; set; }
    }
        public class PostResultSetting
    {
        public Guid ResultSettingId { get; set; }
        public bool PromoteByPassmark { get; set; }
        public bool PromoteAll { get; set; }
        public bool ShowPositionOnResult { get; set; }
        public bool CumulativeResult { get; set; }
        public bool ShowNewsletter { get; set; }
        public bool BatchPrinting { get; set; }
        public IFormFile PrincipalStamp { get; set; }
        public string Filepath { get; set; }
        public string SelectedTemplate { get; set; }
    }
    public class ResultSettingContract
    {
        public Guid ResultSettingId { get; set; }
        public bool PromoteByPassmark { get; set; }
        public bool PromoteAll { get; set; }
        public bool ShowPositionOnResult { get; set; }
        public bool CumulativeResult { get; set; }
        public bool ShowNewsletter { get; set; }
        public bool BatchPrinting { get; set; }
        public string Filepath { get; set; }
        public ResultSettingContract()
        {

        }
        public ResultSettingContract( ResultSetting db)
        {
            ResultSettingId = db.ResultSettingId;
            PromoteByPassmark = db.PromoteByPassmark;
            PromoteAll = db.PromoteAll;
            ShowPositionOnResult = db.ShowPositionOnResult;
            CumulativeResult = db.CumulativeResult;
            ShowNewsletter = db.ShowNewsletter;
            BatchPrinting = db.BatchPrinting;
            Filepath = db.PrincipalStample;
        }
    }
}
