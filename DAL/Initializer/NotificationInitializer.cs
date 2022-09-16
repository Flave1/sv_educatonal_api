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
            var parents = new List<NotificationSetting>
                {
                    new NotificationSetting
                    {
                        NotificationSettingId = Guid.Parse("6f30d20a-8383-42f7-a2a6-8cd0ecdd0086"),
                        Announcement = "false/email",
                        Assessment = "false/email",
                        ClassManagement = "false/email",
                        Enrollment = "false/email",
                        Permission = "false/email",
                        PublishResult = "false/email",
                        RecoverPassword = "false/email",
                        Session = "false/email",
                        ShouldSendToParentsOnResultPublish = false,
                        Staff = "false/email",
                    },

                };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in parents)
                    {
                        if (!context.NotificationSetting.Any(w => w.NotificationSettingId == item.NotificationSettingId))
                        {
                            context.Add(item);
                            context.SaveChanges();
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
