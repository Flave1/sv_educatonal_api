using DAL.ClassEntities;
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
        public string AdmissionSettingName { get; set; }
        public List<AdmissionClasses> Classes { get; set; }
        public bool AdmissionStatus { get; set; }
        public string PassedExamEmail { get; set; }
        public string FailedExamEmail { get; set; }
        public string ScreeningEmail { get; set; }
        public bool RegistrationFee { get; set; }
        public SelectAdmissionSettings(AdmissionSetting settings, List<ClassLookup> classLookup)
        {
            AdmissionSettingId = settings.AdmissionSettingId.ToString();
            AdmissionSettingName = settings.AdmissionSettingName.ToString();
            Classes = classLookup?.Select(x => new AdmissionClasses
            {
                ClassId = x.ClassLookupId.ToString(),
                ClassName = x.Name
            }).ToList();
            AdmissionStatus = settings.AdmissionStatus;
            PassedExamEmail = settings.PassedExamEmail;
            FailedExamEmail = settings.FailedExamEmail;
            ScreeningEmail = settings.ScreeningEmail;
            RegistrationFee = settings.RegistrationFee;
        }
    }
    public class AdmissionClasses
    {
        public string ClassId { get; set; }
        public string ClassName { get; set; }
    }
}
