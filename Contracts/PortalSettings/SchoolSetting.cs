using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class PostSchoolSetting
    {
        public Guid SchoolSettingsId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolAbbreviation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string SchoolType { get; set; }
        public string Filepath { get; set; }
        public IFormFile Photo { get; set; }
        public string Email { get; set; }
    }
        public class SchoolSettingContract
    {
        public Guid SchoolSettingsId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolAbbreviation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string SchoolType { get; set; }
        public string Filepath { get; set; }
        public  string Email { get; set; }
        public SchoolSettingContract()
        {

        }
        public SchoolSettingContract(SchoolSetting db)
        {
            SchoolSettingsId = db.SchoolSettingsId;
            SchoolName = db?.SCHOOLSETTINGS_SchoolName??"";
            SchoolAddress = db?.SCHOOLSETTINGS_SchoolAddress ?? "";
            SchoolAbbreviation = db?.SCHOOLSETTINGS_SchoolAbbreviation ?? "";
            Country = db?.SCHOOLSETTINGS_Country ?? "";
            State = db?.SCHOOLSETTINGS_State ?? "";
            PhoneNo1 = db?.SCHOOLSETTINGS_PhoneNo1 ?? "";
            PhoneNo2 = db?.SCHOOLSETTINGS_PhoneNo2 ?? "";
            SchoolType = db?.SCHOOLSETTINGS_SchoolType ?? "";
            Filepath = db.SCHOOLSETTINGS_Photo;
            Email = db?.SCHOOLSETTINGS_Email ?? "";
        }
    }
}
