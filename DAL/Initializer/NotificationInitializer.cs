using DAL;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Initializer
{

    public static class NotificationInitializer
    {
        //DO NOT TOUCH FILE ################################## DO NOT TOUCH FILE
        internal static void SeedNotificationSetting(DataContext context)
        {
            var parents = new List<SchoolSetting>
                {
                    new SchoolSetting
                    {
                        SchoolSettingsId = Guid.Parse("6f30d20a-8383-42f7-a2a6-8cd0ecdd0086"),
                        NOTIFICATIONSETTINGS_Announcement = "email/false",
                        NOTIFICATIONSETTINGS_Assessment = "email/false",
                        NOTIFICATIONSETTINGS_ClassManagement = "email/false",
                        NOTIFICATIONSETTINGS_Enrollment = "email/false",
                        NOTIFICATIONSETTINGS_Permission = "email/false",
                        NOTIFICATIONSETTINGS_PublishResult = "email/false",
                        NOTIFICATIONSETTINGS_RecoverPassword = "email/false",
                        NOTIFICATIONSETTINGS_Session = "email/false",
                        NOTIFICATIONSETTINGS_ShouldSendToParentsOnResultPublish = false,
                        NOTIFICATIONSETTINGS_Staff = "email/false",
                        ClientId = "D7F710A5-592F-4390-068C-08DA686DA23E"
                    },

                };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in parents)
                    {
                        if (!context.SchoolSettings.Any(w => w.SchoolSettingsId == item.SchoolSettingsId))
                        {
                            context.Add(item);
                            context.SaveChangesNoClientAsync().Wait();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally { transaction.Dispose(); }
            }
        }
    }
}
