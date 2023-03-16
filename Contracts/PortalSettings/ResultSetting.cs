using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public string ResultSettingId { get; set; }
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
        public string ResultSettingId { get; set; }
        public bool PromoteAll { get; set; }
        public bool ShowPositionOnResult { get; set; }
        public bool CumulativeResult { get; set; }
        public bool ShowNewsletter { get; set; }
        public bool BatchPrinting { get; set; }
        public string Filepath { get; set; }
        public string SelectedTemplate { get;set; }
        public string Headteacher { get; set; }
        public ResultSettingContract()
        {

        }
        public ResultSettingContract( SchoolSetting db)
        {
            ResultSettingId = db.SchoolSettingsId.ToString();
            PromoteAll = db.RESULTSETTINGS_PromoteAll;
            ShowPositionOnResult = db.RESULTSETTINGS_ShowPositionOnResult;
            CumulativeResult = db.RESULTSETTINGS_CumulativeResult;
            ShowNewsletter = db.RESULTSETTINGS_ShowNewsletter;
            BatchPrinting = db.RESULTSETTINGS_BatchPrinting;
            Filepath = db.RESULTSETTINGS_PrincipalStample;
            SelectedTemplate = db.RESULTSETTINGS_SelectedTemplate;
        }
    }
}
