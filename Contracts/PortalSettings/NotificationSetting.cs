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
        public PostNotificationSetting(NotificationSetting st)
        {
            RecoverPassword = new RecoverPassword(st.RecoverPassword);
            Announcement = new Announcement(st.Announcement);
            Assessment = new Assessment(st.Assessment);
            Permission = new Permission(st.Permission);
            Session = new Session(st.Session);
            ClassManagement = new ClassManagement(st.ClassManagement);
            Staff = new Staff(st.Staff);
            Enrollment = new Enrollment(st.Enrollment);
            PublishResult = new PublishResult(st.PublishResult, st.ShouldSendToParentsOnResultPublish);
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
        public string sidebarType { get; set; }
        public string sidebarActiveStyle { get; set; }
        public string navbarstyle { get; set; }
        public string loginTemplate { get; set; }

    }
}
