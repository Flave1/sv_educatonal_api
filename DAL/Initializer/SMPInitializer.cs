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
                    new ActivityParent { Id = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"), Name = "session-management", DisplayName = "Session Mangement"},
                    new ActivityParent { Id = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), Name = "class-management", DisplayName = "Class Mangement"},
                    new ActivityParent { Id = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"), Name = "staff-management", DisplayName = "Staff Mangement"},
                    new ActivityParent { Id = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"), Name = "student-management", DisplayName = "Student Mangement"},
                    new ActivityParent { Id = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"), Name = "enrollment-management", DisplayName = "Enrollment Mangement"},
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
                    new Activity { Id = Guid.Parse("dd686758-23e9-48ce-af5b-8f6d1d3fcc34"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "role-list", DisplayName ="All Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("989a045d-6ae4-4091-bd93-f1ce74ca02bc"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "add-role", DisplayName = "Create Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("902d8c32-ab45-44e9-be04-d6b36aeeca97"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "edit-list", DisplayName =  "Update Roles", IsActive = true, Deleted = false},
                    new Activity { Id = Guid.Parse("939122bb-c19a-4694-a040-15f8468dad75"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "add-user-to-role", DisplayName = "Add User To Role", IsActive = true, Deleted = false},


                    //session-management
                    new Activity {
                        Id = Guid.Parse("b6b8897a-5c2c-4a8d-ae47-4f3a4665951e"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-setup",
                        DisplayName = "Session Setup", IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("332fd58d-8664-4d27-9ddf-3617a812de3f"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-class-setup",
                        DisplayName = "Class Setup", IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("c53886aa-e62c-4c96-9473-458d0b3195b1"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-subject-setup",
                        DisplayName = "Subject Setup", IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("b85246c6-700e-4b4d-a21d-b38c90e7c066"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-class",
                        DisplayName = "Class", IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("09d8f4d4-296f-4084-9d32-c2c1c9469779"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-promotion",
                        DisplayName = "Promotion", IsActive = true, Deleted = false},

                    //class-management
                    new Activity { 
                        Id = Guid.Parse("b1a0c6ca-2b0e-41f8-b188-13b7fdaf6e59"),  
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), 
                        Permission = "class-class", 
                        DisplayName = "Class", 
                        IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("79d3a65f-b6ce-42a3-97be-b1ac47fd2327"),
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"),
                        Permission = "class-attendance",
                        DisplayName = "Attendance",
                        IsActive = true, Deleted = false},

                     //staff-management
                    new Activity {
                        Id = Guid.Parse("b1a0c6ca-2b0e-41f8-b188-13b7fdaf6e59"),
                        ActivityParentId = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"),
                        Permission = "staff-all-staff",
                        DisplayName = "All Staff",
                        IsActive = true, Deleted = false},

                     //student-management
                    new Activity {
                        Id = Guid.Parse("06d33b7c-9df0-48a2-884b-928b5ad2e24b"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "student-all-student",
                        DisplayName = "All Student",
                        IsActive = true, Deleted = false},

                    //enrollment-management
                    new Activity {
                        Id = Guid.Parse("7282f924-1449-4774-813c-6d83b7eb1868"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "enrollment-enrolled",
                        DisplayName = "Enrolled Students",
                        IsActive = true, Deleted = false},
                    new Activity {
                        Id = Guid.Parse("17edf1f0-de9c-4ca2-98d3-ef3652d4d088"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "enrollment-unenrolled",
                        DisplayName = "Unenrolled Students",
                        IsActive = true, Deleted = false},


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
