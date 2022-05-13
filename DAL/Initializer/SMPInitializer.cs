using DAL;
using DAL.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.DAL.Initializer
{
    public static class DALInitializers
    {
        //DO NOT TOUCH FILE ################################## DO NOT TOUCH FILE
        internal static void SeedActivityParents(DataContext context)
        {
            //parent activities
            var parents = new List<ActivityParent>
            {
                    new ActivityParent { Id = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"), Name = "dashboard", DisplayName = "Dashboard"},
                    new ActivityParent { Id = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "permissions-management", DisplayName = "Permissions Management"},
                    new ActivityParent { Id = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), Name = "class-management", DisplayName = "Class Mangement"},
            };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in parents)
                    {
                        if (!context.ActivityParent.Any(w => w.Id == item.Id))
                        {
                            context.Add(item);
                            context.SaveChanges();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally { transaction.Dispose(); }
            }
        }
        internal static void SeedActivities(DataContext context)
        {
            var activities = new List<Activity>
            {
                    //permissions-management
                    new Activity { Id = Guid.Parse("dd686758-23e9-48ce-af5b-8f6d1d3fcc34"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "role-list", DisplayName ="All Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("989a045d-6ae4-4091-bd93-f1ce74ca02bc"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "add-role", DisplayName = "Create Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("902d8c32-ab45-44e9-be04-d6b36aeeca97"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "edit-list", DisplayName =  "Update Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("939122bb-c19a-4694-a040-15f8468dad75"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "add-user-to-role", DisplayName = "Add User To Role", IsActive = true, Deleted = false},


                    //class-management
                    new Activity { Id = Guid.Parse("b1a0c6ca-2b0e-41f8-b188-13b7fdaf6e59"),  ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), Name = "class-name-setup", DisplayName = "Class Setup", IsActive = true, Deleted = false}
            };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in activities)
                    {
                        if (!context.Activity.Any(w => w.Id == item.Id))
                        {
                            context.Add(item);
                            context.SaveChanges();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally { transaction.Dispose(); }
            }
        }

    }

}
