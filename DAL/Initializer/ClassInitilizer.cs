using DAL;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.DAL.Initializer
{
    public static class ClassInitilizer
    {
            //DO NOT TOUCH FILE ################################## DO NOT TOUCH FILE
            internal static void SeedClassGroup(DataContext context)
            {
                //parent activities
                var parents = new List<SessionClassGroup>
                {
                    new SessionClassGroup 
                    { 
                        SessionClassGroupId = Guid.Parse("eba102ba-d96c-4920-812a-080c8fdbe767"),
                        GroupName = "all-students",
                        ClientId = "D7F710A5-592F-4390-068C-08DA686DA23E"
                    },
                 
                };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in parents)
                        {
                            if (!context.SessionClassGroup.Any(w => w.SessionClassGroupId == item.SessionClassGroupId))
                            {
                                context.Add(item);
                                context.SaveChangesNoClientAsync().Wait();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally { transaction.Dispose(); }
                }
            }
        }
}
