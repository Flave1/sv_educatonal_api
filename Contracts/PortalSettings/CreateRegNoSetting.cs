using DAL;
using SMP.DAL.Migrations;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PortalSettings
{
    public class CreateRegNoSetting
    {
        public string StudentRegNoPrefix { get; set; }
        public string StudentRegNoSufix { get; set; }
        public string TeacherRegNoPrefix { get; set; }
        public string TeacherRegNoSufix { get; set; }
        public int RegNoPosition { get; set; }
        public string RegNoSeperator { get; set; }
    }

    public class PortalSettingDto
    {
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
        public PortalSettingDto(SchoolSetting setting, string type = "")
        {
            if(!string.IsNullOrEmpty(type))
            {
                APPLAYOUTSETTINGS_Scheme = setting.APPLAYOUTSETTINGS_Scheme;
                APPLAYOUTSETTINGS_SchemeDir = setting.APPLAYOUTSETTINGS_SchemeDir;
                APPLAYOUTSETTINGS_Colorprimary = setting.APPLAYOUTSETTINGS_Colorprimary;
                APPLAYOUTSETTINGS_Navbarstyle = setting.APPLAYOUTSETTINGS_Navbarstyle;
                APPLAYOUTSETTINGS_Sidebarcolor = setting.APPLAYOUTSETTINGS_Sidebarcolor;
                APPLAYOUTSETTINGS_Colorcustomizer = setting.APPLAYOUTSETTINGS_Colorcustomizer;
                APPLAYOUTSETTINGS_Colorinfo = setting.APPLAYOUTSETTINGS_Colorinfo;
                APPLAYOUTSETTINGS_loginTemplate = setting.APPLAYOUTSETTINGS_loginTemplate;
                APPLAYOUTSETTINGS_SidebarActiveStyle = setting.APPLAYOUTSETTINGS_SidebarActiveStyle;
                APPLAYOUTSETTINGS_SchoolUrl = setting.APPLAYOUTSETTINGS_SchoolUrl;

                NOTIFICATIONSETTINGS_RecoverPassword = setting.NOTIFICATIONSETTINGS_RecoverPassword;
                NOTIFICATIONSETTINGS_Announcement = setting.NOTIFICATIONSETTINGS_Announcement;
                NOTIFICATIONSETTINGS_Assessment = setting.NOTIFICATIONSETTINGS_Assessment;
                NOTIFICATIONSETTINGS_Permission = setting.NOTIFICATIONSETTINGS_Permission;
                NOTIFICATIONSETTINGS_Session = setting.NOTIFICATIONSETTINGS_Session;
                NOTIFICATIONSETTINGS_ClassManagement = setting.NOTIFICATIONSETTINGS_ClassManagement;
                NOTIFICATIONSETTINGS_Staff = setting.NOTIFICATIONSETTINGS_Staff;
                NOTIFICATIONSETTINGS_Enrollment = setting.NOTIFICATIONSETTINGS_Enrollment;
                NOTIFICATIONSETTINGS_PublishResult = setting.NOTIFICATIONSETTINGS_PublishResult;
                NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = setting.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish;
                RESULTSETTINGS_PromoteAll = setting.RESULTSETTINGS_PromoteAll;
                RESULTSETTINGS_ShowPositionOnResult = setting.RESULTSETTINGS_ShowPositionOnResult;
                RESULTSETTINGS_CumulativeResult = setting.RESULTSETTINGS_CumulativeResult;
                RESULTSETTINGS_ShowNewsletter = setting.RESULTSETTINGS_ShowNewsletter;
                RESULTSETTINGS_BatchPrinting = setting.RESULTSETTINGS_BatchPrinting;
                RESULTSETTINGS_PrincipalStample = setting.RESULTSETTINGS_PrincipalStample;
                RESULTSETTINGS_SelectedTemplate = setting.RESULTSETTINGS_SelectedTemplate;

                SCHOOLSETTINGS_SchoolName = setting.SCHOOLSETTINGS_SchoolName;
                SCHOOLSETTINGS_SchoolAbbreviation = setting.SCHOOLSETTINGS_SchoolAbbreviation;
                SCHOOLSETTINGS_SchoolAddress = setting.SCHOOLSETTINGS_SchoolAddress;
                SCHOOLSETTINGS_Country = setting.SCHOOLSETTINGS_Country;
                SCHOOLSETTINGS_State = setting.SCHOOLSETTINGS_State;
                SCHOOLSETTINGS_Email = setting.SCHOOLSETTINGS_Email;
                SCHOOLSETTINGS_PhoneNo1 = setting.SCHOOLSETTINGS_PhoneNo1;
                SCHOOLSETTINGS_PhoneNo2 = setting.SCHOOLSETTINGS_PhoneNo2;
                SCHOOLSETTINGS_SchoolType = setting.SCHOOLSETTINGS_SchoolType;
                SCHOOLSETTINGS_Photo = setting.SCHOOLSETTINGS_Photo;
                SCHOOLSETTINGS_StudentRegNoFormat = setting.SCHOOLSETTINGS_StudentRegNoFormat;
                SCHOOLSETTINGS_TeacherRegNoFormat = setting.SCHOOLSETTINGS_TeacherRegNoFormat;
                SCHOOLSETTINGS_RegNoPosition = setting.SCHOOLSETTINGS_RegNoPosition;
                SCHOOLSETTINGS_RegNoSeperator = setting.SCHOOLSETTINGS_RegNoSeperator;
            }

            if(type == "layout")
            {
                APPLAYOUTSETTINGS_Scheme = setting.APPLAYOUTSETTINGS_Scheme;
                APPLAYOUTSETTINGS_SchemeDir = setting.APPLAYOUTSETTINGS_SchemeDir;
                APPLAYOUTSETTINGS_Colorprimary = setting.APPLAYOUTSETTINGS_Colorprimary;
                APPLAYOUTSETTINGS_Navbarstyle = setting.APPLAYOUTSETTINGS_Navbarstyle;
                APPLAYOUTSETTINGS_Sidebarcolor = setting.APPLAYOUTSETTINGS_Sidebarcolor;
                APPLAYOUTSETTINGS_Colorcustomizer = setting.APPLAYOUTSETTINGS_Colorcustomizer;
                APPLAYOUTSETTINGS_Colorinfo = setting.APPLAYOUTSETTINGS_Colorinfo;
                APPLAYOUTSETTINGS_loginTemplate = setting.APPLAYOUTSETTINGS_loginTemplate;
                APPLAYOUTSETTINGS_SidebarActiveStyle = setting.APPLAYOUTSETTINGS_SidebarActiveStyle;
                APPLAYOUTSETTINGS_SchoolUrl = setting.APPLAYOUTSETTINGS_SchoolUrl;
            }

            if (type == "notify")
            {
                NOTIFICATIONSETTINGS_RecoverPassword = setting.NOTIFICATIONSETTINGS_RecoverPassword;
                NOTIFICATIONSETTINGS_Announcement = setting.NOTIFICATIONSETTINGS_Announcement;
                NOTIFICATIONSETTINGS_Assessment = setting.NOTIFICATIONSETTINGS_Assessment;
                NOTIFICATIONSETTINGS_Permission = setting.NOTIFICATIONSETTINGS_Permission;
                NOTIFICATIONSETTINGS_Session = setting.NOTIFICATIONSETTINGS_Session;
                NOTIFICATIONSETTINGS_ClassManagement = setting.NOTIFICATIONSETTINGS_ClassManagement;
                NOTIFICATIONSETTINGS_Staff = setting.NOTIFICATIONSETTINGS_Staff;
                NOTIFICATIONSETTINGS_Enrollment = setting.NOTIFICATIONSETTINGS_Enrollment;
                NOTIFICATIONSETTINGS_PublishResult = setting.NOTIFICATIONSETTINGS_PublishResult;
                NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = setting.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish;
                RESULTSETTINGS_PromoteAll = setting.RESULTSETTINGS_PromoteAll;
                RESULTSETTINGS_ShowPositionOnResult = setting.RESULTSETTINGS_ShowPositionOnResult;
                RESULTSETTINGS_CumulativeResult = setting.RESULTSETTINGS_CumulativeResult;
                RESULTSETTINGS_ShowNewsletter = setting.RESULTSETTINGS_ShowNewsletter;
                RESULTSETTINGS_BatchPrinting = setting.RESULTSETTINGS_BatchPrinting;
                RESULTSETTINGS_PrincipalStample = setting.RESULTSETTINGS_PrincipalStample;
                RESULTSETTINGS_SelectedTemplate = setting.RESULTSETTINGS_SelectedTemplate;
            }

            if(type == "setting")
            {
                NOTIFICATIONSETTINGS_RecoverPassword = setting.NOTIFICATIONSETTINGS_RecoverPassword;
                NOTIFICATIONSETTINGS_Announcement = setting.NOTIFICATIONSETTINGS_Announcement;
                NOTIFICATIONSETTINGS_Assessment = setting.NOTIFICATIONSETTINGS_Assessment;
                NOTIFICATIONSETTINGS_Permission = setting.NOTIFICATIONSETTINGS_Permission;
                NOTIFICATIONSETTINGS_Session = setting.NOTIFICATIONSETTINGS_Session;
                NOTIFICATIONSETTINGS_ClassManagement = setting.NOTIFICATIONSETTINGS_ClassManagement;
                NOTIFICATIONSETTINGS_Staff = setting.NOTIFICATIONSETTINGS_Staff;
                NOTIFICATIONSETTINGS_Enrollment = setting.NOTIFICATIONSETTINGS_Enrollment;
                NOTIFICATIONSETTINGS_PublishResult = setting.NOTIFICATIONSETTINGS_PublishResult;
                NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = setting.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish;
                RESULTSETTINGS_PromoteAll = setting.RESULTSETTINGS_PromoteAll;
                RESULTSETTINGS_ShowPositionOnResult = setting.RESULTSETTINGS_ShowPositionOnResult;
                RESULTSETTINGS_CumulativeResult = setting.RESULTSETTINGS_CumulativeResult;
                RESULTSETTINGS_ShowNewsletter = setting.RESULTSETTINGS_ShowNewsletter;
                RESULTSETTINGS_BatchPrinting = setting.RESULTSETTINGS_BatchPrinting;
                RESULTSETTINGS_PrincipalStample = setting.RESULTSETTINGS_PrincipalStample;
                RESULTSETTINGS_SelectedTemplate = setting.RESULTSETTINGS_SelectedTemplate;
            }
        }
    }
}
