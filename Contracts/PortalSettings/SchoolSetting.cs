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
        public SchoolSettingContract()
        {

        }
        public SchoolSettingContract(SchoolSetting db)
        {
            SchoolSettingsId = db.SchoolSettingsId;
            SchoolName = db.SchoolName;
            SchoolAddress = db.SchoolAddress;
            SchoolAbbreviation = db.SchoolAbbreviation;
            Country = db.Country;
            State = db.State;
            PhoneNo1 = db.PhoneNo1;
            PhoneNo2 = db.PhoneNo2;
            SchoolType = db.SchoolType;
            Filepath = db.Photo;
        }
    }
}
