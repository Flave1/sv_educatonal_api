﻿using SMP.DAL.Models.PortalSettings;
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
    }
        public class SchoolSettingContract
    {
        public Guid SchoolSettingsId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolAbbreviation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Phone_no1 { get; set; }
        public string Phone_no2 { get; set; }
        public string SchoolType { get; set; } 
        public SchoolSettingContract(SchoolSetting db)
        {
            SchoolSettingsId = db.SchoolSettingsId;
            SchoolName = db.SchoolName;
            SchoolAddress = db.SchoolAddress;
            SchoolAbbreviation = db.SchoolAbbreviation;
            Country = db.Country;
            State = db.State;
            Phone_no1 = db.PhoneNo1;
            Phone_no2 = db.PhoneNo2;
            SchoolType = db.SchoolType; 
        }
    }
}
