﻿using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class RegNoSetting
    {
        public string SchoolSettingsId { get; set; }
        public string StudentRegNoFormat { get; set; }
        public string TeacherRegNoFormat { get; set; }
        public string RegNoPosition { get; set; }
        public string RegNoSeperator { get; set; }
        public RegNoSetting()
        {

        }
        public RegNoSetting(SchoolSetting schoolSetting)
        {
            SchoolSettingsId = schoolSetting.SchoolSettingsId.ToString();
            StudentRegNoFormat = schoolSetting?.SCHOOLSETTINGS_StudentRegNoFormat ?? "";
            TeacherRegNoFormat = schoolSetting?.SCHOOLSETTINGS_TeacherRegNoFormat ?? "";
            RegNoPosition = schoolSetting?.SCHOOLSETTINGS_RegNoPosition.ToString() ?? "";
            RegNoSeperator = schoolSetting?.SCHOOLSETTINGS_RegNoSeperator ?? "";
        }
    }
}
