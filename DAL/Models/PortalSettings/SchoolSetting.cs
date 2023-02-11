using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PortalSettings
{
    public class SchoolSetting : CommonEntity
    {
        [Key]
        public Guid SchoolSettingsId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolAbbreviation { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string SchoolType  { get; set; }
        public string Photo { get; set; }
        public string StudentRegNoFormat { get; set; }
        public string TeacherRegNoFormat { get; set; }
        public int? RegNoPosition { get; set; }
        public string RegNoSeperator { get; set; }
    }
}
