using SMP.DAL.Models.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.AdmissionSettings
{
    public class SelectAdmissionSettings
    {
        public string AdmissionSettingId { get; set; }
        public string[] Classes { get; set; }
        public int AdmissionStatus { get; set; }
        public string PassedExamEmail { get; set; }
        public string FailedExamEmail { get; set; }
        public string ScreeningEmail { get; set; }
        public bool RegistrationFee { get; set; }
        public SelectAdmissionSettings(AdmissionSetting settings)
        {
            AdmissionSettingId = settings.AdmissionSettingId.ToString();
            Classes = settings.Classes.Split(",");
            AdmissionStatus = settings.AdmissionStatus;
            PassedExamEmail = settings.PassedExamEmail;
            FailedExamEmail = settings.FailedExamEmail;
            ScreeningEmail = settings.ScreeningEmail;
            RegistrationFee = settings.RegistrationFee;
        }
    }
}
