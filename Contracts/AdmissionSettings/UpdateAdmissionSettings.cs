using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.AdmissionSettings
{
    public class UpdateAdmissionSettings
    {
        public string AdmissionSettingName { get; set; }
        public string AdmissionSettingsId { get; set; }
        public string[] Classes { get; set; }
        public bool AdmissionStatus { get; set; }
        public string PassedExamEmail { get; set; }
        public string FailedExamEmail { get; set; }
        public string ScreeningEmail { get; set; }
        public bool RegistrationFee { get; set; }
    }
}
