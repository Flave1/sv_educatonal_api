using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PortalSettings
{
    public class SchoolSetting : CommonEntity
    {
        [Key]
        public Guid SchoolSettingsId { get; set; }
        public string SCHOOLSETTINGS_SchoolName { get; set; }
        public string SCHOOLSETTINGS_SchoolAddress { get; set; }
        public string SCHOOLSETTINGS_SchoolAbbreviation { get; set; }
        public string SCHOOLSETTINGS_Country { get; set; }
        public string SCHOOLSETTINGS_State { get; set; }
        public string SCHOOLSETTINGS_Email { get; set; }
        public string SCHOOLSETTINGS_PhoneNo1 { get; set; }
        public string SCHOOLSETTINGS_PhoneNo2 { get; set; }
        public string SCHOOLSETTINGS_SchoolType { get; set; }
        public string SCHOOLSETTINGS_Photo { get; set; }
        public string SCHOOLSETTINGS_StudentRegNoFormat { get; set; }
        public string SCHOOLSETTINGS_TeacherRegNoFormat { get; set; }
        public int? SCHOOLSETTINGS_RegNoPosition { get; set; }
        public string SCHOOLSETTINGS_RegNoSeperator { get; set; }
        public string APPLAYOUTSETTINGS_Scheme { get; set; }
        public string APPLAYOUTSETTINGS_Colorcustomizer { get; set; }
        public string APPLAYOUTSETTINGS_Colorinfo { get; set; }
        public string APPLAYOUTSETTINGS_Colorprimary { get; set; }
        public string APPLAYOUTSETTINGS_SchemeDir { get; set; }
        public string APPLAYOUTSETTINGS_Sidebarcolor { get; set; }
        public string APPLAYOUTSETTINGS_SidebarType { get; set; }
        public string APPLAYOUTSETTINGS_SidebarActiveStyle { get; set; }
        public string APPLAYOUTSETTINGS_Navbarstyle { get; set; }
        public string APPLAYOUTSETTINGS_loginTemplate { get; set; }
        public string APPLAYOUTSETTINGS_SchoolUrl { get; set; }
        public string NOTIFICATIONSETTINGS_RecoverPassword { get; set; }
        public string NOTIFICATIONSETTINGS_Announcement { get; set; }
        public string NOTIFICATIONSETTINGS_Assessment { get; set; }
        public string NOTIFICATIONSETTINGS_Permission { get; set; }
        public string NOTIFICATIONSETTINGS_Session { get; set; }
        public string NOTIFICATIONSETTINGS_ClassManagement { get; set; }
        public string NOTIFICATIONSETTINGS_Staff { get; set; }
        public string NOTIFICATIONSETTINGS_Enrollment { get; set; }
        public string NOTIFICATIONSETTINGS_PublishResult { get; set; }
        public bool NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish { get; set; }
        public bool RESULTSETTINGS_PromoteAll { get; set; }
        public bool RESULTSETTINGS_ShowPositionOnResult { get; set; }
        public bool RESULTSETTINGS_CumulativeResult { get; set; }
        public bool RESULTSETTINGS_ShowNewsletter { get; set; }
        public bool RESULTSETTINGS_BatchPrinting { get; set; }
        public string RESULTSETTINGS_PrincipalStample { get; set; }
        public string RESULTSETTINGS_SelectedTemplate { get; set; }
    }
}
