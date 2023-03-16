using SMP.DAL.Models.PortalSettings;
using System;
using System.Linq;

namespace SMP.Contracts.PortalSettings
{
    public class RecoverPassword : NotificationSesstionMedia
    {
        public RecoverPassword()
        {

        }
        public RecoverPassword(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Announcement : NotificationSesstionMedia
    {
        public Announcement()
        {

        }
        public Announcement(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Assessment : NotificationSesstionMedia
    {
        public Assessment()
        {

        }
        public Assessment(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Permission : NotificationSesstionMedia
    {
        public Permission()
        {

        }
        public Permission(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Session : NotificationSesstionMedia
    {
        public Session()
        {

        }
        public Session(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class ClassManagement : NotificationSesstionMedia
    {
        public ClassManagement()
        {

        }
        public ClassManagement(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Staff : NotificationSesstionMedia
    {
        public Staff()
        {

        }
        public Staff(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }
    public class Enrollment : NotificationSesstionMedia
    {
        public Enrollment()
        {

        }
        public Enrollment(string setting)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
        }
    }

    public class PublishResult : NotificationSesstionMedia
    {
        public PublishResult()
        {

        }
        public bool ShouldSendToParentsOnResultPublish { get; set; }
        public PublishResult(string setting, bool shouldSendToParentsOnResultPublish)
        {
            var splitted = setting.Split('/').ToArray();
            media = splitted[0];
            send = splitted[1] == "True" ? true : false;
            ShouldSendToParentsOnResultPublish = shouldSendToParentsOnResultPublish;
        }
    }

    public class NotificationSesstionMedia
    {
        public string media { get; set; }
        public bool send { get; set; }

    }
    public class PostNotificationSetting
    {
        public RecoverPassword RecoverPassword { get; set; } = new RecoverPassword();
        public Announcement Announcement { get; set; } = new Announcement();
        public Assessment Assessment { get; set; } = new Assessment();
        public Permission Permission { get; set; } = new Permission();
        public Session Session { get; set; } = new Session();
        public ClassManagement ClassManagement { get; set; } = new ClassManagement();
        public Staff Staff { get; set; } = new Staff();
        public Enrollment Enrollment { get; set; } = new Enrollment();
        public PublishResult PublishResult { get; set; } = new PublishResult();
        public PostNotificationSetting()
        {

        }
        public PostNotificationSetting(SchoolSetting st)
        {
            RecoverPassword = new RecoverPassword(st.NOTIFICATIONSETTINGS_RecoverPassword);
            Announcement = new Announcement(st.NOTIFICATIONSETTINGS_Announcement);
            Assessment = new Assessment(st.NOTIFICATIONSETTINGS_Assessment);
            Permission = new Permission(st.NOTIFICATIONSETTINGS_Permission);
            Session = new Session(st.NOTIFICATIONSETTINGS_Session);
            ClassManagement = new ClassManagement(st.NOTIFICATIONSETTINGS_ClassManagement);
            Staff = new Staff(st.NOTIFICATIONSETTINGS_Staff);
            Enrollment = new Enrollment(st.NOTIFICATIONSETTINGS_Enrollment);
            PublishResult = new PublishResult(st.NOTIFICATIONSETTINGS_PublishResult, st.NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish);
        }
    }

    public class AppLayoutSettings
    {
        public string scheme { get; set; }
        public string colorcustomizer { get; set; }
        public string colorinfo { get; set; }
        public string colorprimary { get; set; }
        public string schemeDir { get; set; }
        public string sidebarcolor { get; set; }
        public SidebarType sidebarType { get; set; }
        public string sidebarActiveStyle { get; set; }
        public string navbarstyle { get; set; }
        public string loginTemplate { get; set; }
        public string schoolUrl { get; set; }

    }
    public class SidebarType
    {
        public string Mini { get; set; } = "";
        public string Hover { get; set; } = "";
        public string Boxed { get; set; } = "";
    }
    public class SMSSMPAccountSetting
    {
        public string ClientId { get; set; }
        public string SchoolName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string SchoolLogo { get; set; }
        public SMSSMPAccountSetting(string SchoolName, string Country, string State, string Address, string SchoolLogo, string clientId)
        {
            this.SchoolName = SchoolName;
            this.Country = Country;
            this.State = State;
            this.Address = Address;
            this.SchoolLogo = SchoolLogo;
            ClientId = clientId;
        }
    }
}
