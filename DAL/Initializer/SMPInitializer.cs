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
            var parents = new List<AppActivityParent>
            {
                    new AppActivityParent { Id = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"), Name = "dashboard", DisplayName = "Dashboard"},
                    new AppActivityParent { Id = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Name = "permissions-management", DisplayName = "Permissions Management"},
                    new AppActivityParent { Id = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"), Name = "session-management", DisplayName = "Session Mangement"},
                    new AppActivityParent { Id = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), Name = "class-management", DisplayName = "Class Mangement"},
                    new AppActivityParent { Id = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"), Name = "staff-management", DisplayName = "Staff Mangement"},
                    new AppActivityParent { Id = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"), Name = "student-management", DisplayName = "Student Mangement"},
                    new AppActivityParent { Id = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"), Name = "reult-management", DisplayName = "Result Mangement"},
                    new AppActivityParent { Id = Guid.Parse("17524c4b-119e-4b9f-ac30-734018a000d6"), Name = "pin-management", DisplayName = "Pin Mangement"},
                    new AppActivityParent { Id = Guid.Parse("65e70bfb-b760-41ae-86e1-14cde57dc21c"), Name = "app-settings", DisplayName = "App Settings"},
                    new AppActivityParent { Id = Guid.Parse("b63e6393-52ce-4ffc-8e9e-44a164c1030d"), Name = "Announcement", DisplayName = "Announcement"},
            };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in parents)
                    {
                        if (!context.AppActivityParent.Any(w => w.Id == item.Id))
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
            var activities = new List<AppActivity>
            {
                 //permissions-management
                    new AppActivity { 
                        Id = Guid.Parse("4c97b68a-66ae-45ba-9207-60f367a3376e"),  
                        ActivityParentId = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"), 
                        Permission = "total-enrolled-student", 
                        DisplayName ="Total Enrolled Students", 
                        IsActive = true, 
                        Deleted = false
                    },
                    new AppActivity {
                        Id = Guid.Parse("4d256818-a194-4909-8043-1a04d0e3e7d0"),
                        ActivityParentId = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"),
                        Permission = "total-staff",
                        DisplayName ="Total Staff",
                        IsActive = true,
                        Deleted = false
                    },
                    new AppActivity {
                        Id = Guid.Parse("4e0a0ef8-73ce-435e-b999-40dbc3bc6efa"),
                        ActivityParentId = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"),
                        Permission = "total-classes",
                        DisplayName ="Total Classes",
                        IsActive = true,
                        Deleted = false
                    },
                     new AppActivity {
                        Id = Guid.Parse("d6360937-c0ec-42d8-aba3-abdea50e26a6"),
                        ActivityParentId = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"),
                        Permission = "total-subjects",
                        DisplayName ="Total Subjects",
                        IsActive = true,
                        Deleted = false
                    },
                       new AppActivity {
                        Id = Guid.Parse("6eb800e5-2704-4e83-8f02-b481b71e8581"),
                        ActivityParentId = Guid.Parse("e984b044-d8f5-4ef3-b54a-50cd036426b7"),
                        Permission = "total-pins",
                        DisplayName ="Total Pins",
                        IsActive = true,
                        Deleted = false
                    },


                    //permissions-management
                    new AppActivity { Id = Guid.Parse("dd686758-23e9-48ce-af5b-8f6d1d3fcc34"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "role-list", DisplayName ="All Roles", IsActive = true, Deleted = false},
                    new AppActivity { Id = Guid.Parse("989a045d-6ae4-4091-bd93-f1ce74ca02bc"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "new-role", DisplayName = "New Roles", IsActive = true, Deleted = false},
                    new AppActivity { Id = Guid.Parse("902d8c32-ab45-44e9-be04-d6b36aeeca97"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "edit-role", DisplayName =  "Edit Role", IsActive = true, Deleted = false},
                    new AppActivity { Id = Guid.Parse("939122bb-c19a-4694-a040-15f8468dad75"),  ActivityParentId = Guid.Parse("6cf6e631-edee-49b4-a9d3-d2011f30697c"), Permission = "add-user", DisplayName = "Add User To Role", IsActive = true, Deleted = false},


                    //session-management
                    new AppActivity {
                        Id = Guid.Parse("b6b8897a-5c2c-4a8d-ae47-4f3a4665951e"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-setup",
                        DisplayName = "Session Setup", IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("63d4bb78-7af5-4345-8e42-046e91b8cfce"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "add-session-setup",
                        DisplayName = "Can Create New Session", IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("45237ade-4f4b-4aa2-b7fe-a21471899bfd"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "switch-terms",
                        DisplayName = "Can Switch Terms", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("332fd58d-8664-4d27-9ddf-3617a812de3f"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "class-setup",
                        DisplayName = "Class Setup", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("fe032b87-4a2e-451a-ab35-c815c4356a27"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "create-class-setup",
                        DisplayName = "Can Create Class Setup", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("4f9cff7f-f775-4c22-82db-5fbb4f1906ff"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "edit-class-setup",
                        DisplayName = "Can Edit Class Setup", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("8672c6e1-e304-42ec-afaf-b6472294cf51"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "delete-class-setup",
                        DisplayName = "Can Delete Class Setup", IsActive = true, Deleted = false},
                    
                new AppActivity {
                        Id = Guid.Parse("c53886aa-e62c-4c96-9473-458d0b3195b1"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "subject-setup",
                        DisplayName = "Subject Setup", IsActive = true, Deleted = false},
                new AppActivity {
                        Id = Guid.Parse("8efe96bb-ea20-48ed-963b-5ebcb40a31fe"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "create-subject-setup",
                        DisplayName = "Can Create Subject Setup", IsActive = true, Deleted = false},
                new AppActivity {
                        Id = Guid.Parse("df287d8f-a46a-4d43-8c04-0a3409378071"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "edit-subject-setup",
                        DisplayName = "Can Edit Subject Setup", IsActive = true, Deleted = false},
                new AppActivity {
                        Id = Guid.Parse("d8c6c2fa-093b-4c3e-aba7-1768106b2e15"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "delete-subject-setup",
                        DisplayName = "Can Delete Subject Setup", IsActive = true, Deleted = false},

                    new AppActivity {
                        Id = Guid.Parse("f0199b7e-1427-4899-b852-04f79c3b099a"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "session-session-class",
                        DisplayName = "Class List", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("0dec4f8b-f41a-4630-a45d-46bf2aba0c29"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "create-session-class",
                        DisplayName = "Can Create Session Class", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("29ca873a-c310-4b0a-8b25-e2b8c99c6dca"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "edit-session-class",
                        DisplayName = "Can Edit Session Class", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("2854c952-28d0-49b5-bbca-4d01caf9d905"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "delete-session-class",
                        DisplayName = "Can Delete Session Class", IsActive = true, Deleted = false},

                    new AppActivity {
                        Id = Guid.Parse("09d8f4d4-296f-4084-9d32-c2c1c9469779"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "promotion-list",
                        DisplayName = "Promotion", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("fe53c13e-9c95-4eea-a995-1797ed54c28f"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "promote-students",
                        DisplayName = "Can Promote Students", IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("49697613-386f-4639-b10c-4f07839f48e5"),
                        ActivityParentId = Guid.Parse("93f95f2d-0601-4710-971e-6284ee0e01e3"),
                        Permission = "delete-session",
                        DisplayName = "Can Delete Session", IsActive = true, Deleted = false},


                    //class-management
                    new AppActivity { 
                        Id = Guid.Parse("b1a0c6ca-2b0e-41f8-b188-13b7fdaf6e59"),  
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"), 
                        Permission = "session-class", 
                        DisplayName = "Class", 
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("79d3a65f-b6ce-42a3-97be-b1ac47fd2327"),
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"),
                        Permission = "class-attendance",
                        DisplayName = "Attendance",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("ed2cc466-89c7-4ba5-999a-b7c6f7ca3614"),
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"),
                        Permission = "delete-class-register",
                        DisplayName = "Can Delete Class Register",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("1b7f2fa7-cb3f-4400-aafa-b43f68ddd854"),
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"),
                        Permission = "create-time-table",
                        DisplayName = "Can Create Time Table",
                        IsActive = true, Deleted = false},
                       new AppActivity {
                        Id = Guid.Parse("13f6763e-e5ab-45a0-8da1-244dbe63d298"),
                        ActivityParentId = Guid.Parse("7f70b61e-4dac-465c-8ad5-909c8a822c24"),
                        Permission = "review-lesson-note",
                        DisplayName = "Can Review Lesson Notes",
                        IsActive = true, Deleted = false},

                     //staff-management
                    new AppActivity {
                        Id = Guid.Parse("e124e4ef-1bf7-4ab7-94f2-e58096afd816"),
                        ActivityParentId = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"),
                        Permission = "staff-list",
                        DisplayName = "All Staff",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("7516fc40-373c-4f2e-8a93-165f93d62375"),
                        ActivityParentId = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"),
                        Permission = "create-staff",
                        DisplayName = "Can Create New Staff",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("b7326f27-f504-4ef5-a3db-a43b9e8a3980"),
                        ActivityParentId = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"),
                        Permission = "edit-staff",
                        DisplayName = "Can Edit Staff Information",
                        IsActive = true, Deleted = false},
                       new AppActivity {
                        Id = Guid.Parse("ebe07f38-7809-437d-a6de-ce6f418f31fd"),
                        ActivityParentId = Guid.Parse("c27936ff-87be-4915-8f95-a5ce6a5cdfb9"),
                        Permission = "delete-staff",
                        DisplayName = "Can Delete Staff Information",
                        IsActive = true, Deleted = false},

                     //student-management
                    new AppActivity {
                        Id = Guid.Parse("06d33b7c-9df0-48a2-884b-928b5ad2e24b"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "student-list",
                        DisplayName = "All Student",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("32603911-811a-47da-a491-8052836593b5"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "create-student",
                        DisplayName = "Can Create New Student",
                        IsActive = true, Deleted = false},
                        new AppActivity {
                        Id = Guid.Parse("01b03fa1-7943-4cac-8cd5-05b838d054da"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "edit-student",
                        DisplayName = "Can Edit Student Information",
                        IsActive = true, Deleted = false},
                          new AppActivity {
                        Id = Guid.Parse("4daba8bb-cd66-4a9a-aa2b-b8d63ff31f8c"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "delete-student",
                        DisplayName = "Can Delete Student Information",
                        IsActive = true, Deleted = false},

                    new AppActivity {
                        Id = Guid.Parse("7282f924-1449-4774-813c-6d83b7eb1868"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "enrolled-students-list",
                        DisplayName = "Enrolled Students List",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("ebb2e1fc-e9c4-4247-a092-f9dfc8d961e7"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "enroll-students",
                        DisplayName = "Can Enroll Students",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("17edf1f0-de9c-4ca2-98d3-ef3652d4d088"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "unenrolled-students-list",
                        DisplayName = "Unenrolled Students List",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("2a96ef32-dc12-4bd7-9e69-1485f1169bbb"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "unenroll-students",
                        DisplayName = "Can Unenroll Students",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("f6d56f42-90a0-4e84-a024-9eb713bfeeb4"),
                        ActivityParentId = Guid.Parse("a29a2a62-c814-4c06-bf79-cba341b1043c"),
                        Permission = "change-session-in-unenrolled",
                        DisplayName = "Can View Students In Previous Sessions",
                        IsActive = true, Deleted = false},

                     //result-management
                    new AppActivity {
                        Id = Guid.Parse("b9aace7b-523e-424e-b6a9-6c4966a0f6f1"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "score-entry",
                        DisplayName = "Can Enter Student Scores",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("256ca681-de70-4ece-ac6d-96da5c9a4e7f"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "previous-terms-scores",
                        DisplayName = "Can Update Previous Terms Scores",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("e83e3b04-c322-41f6-b437-bcc05064ef4c"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "view-results-to-publish",
                        DisplayName = "Can View Results To Publish",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("f4b8a11d-2c3b-4a33-8d5c-76abe2f8a3a8"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "publish-result",
                        DisplayName = "Can Publish Results",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("bd5dbe75-3a2e-425c-903d-09776e2d0af2"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "master-list",
                        DisplayName = "Can View Master List",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("1481cdcd-ea1e-4690-a36e-c899c35c4d07"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "export-master-list",
                        DisplayName = "Can Export Master List",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("f79340fc-a4fa-47dc-8ab2-d4e1e0f40798"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "print-result",
                        DisplayName = "Can Print Students Result",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("91e2adbd-c890-487c-8429-3b1136061e68"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "cummulative-master-list",
                        DisplayName = "Can View Cummulative Master List",
                        IsActive = true, Deleted = false},
                    new AppActivity {
                        Id = Guid.Parse("92304cbc-1664-4bc3-b1e2-5c113abba137"),
                        ActivityParentId = Guid.Parse("6ac6237e-63fe-4860-8f29-32151b5fce7d"),
                        Permission = "export-cummulative-master",
                        DisplayName = "Can Export Cummulative Master List",
                        IsActive = true, Deleted = false},


                    //result-management
                    new AppActivity {
                        Id = Guid.Parse("8fcbf7e0-ee97-4669-9697-122f38a5dc26"),
                        ActivityParentId = Guid.Parse("17524c4b-119e-4b9f-ac30-734018a000d6"),
                        Permission = "unused-pins",
                        DisplayName = "Unused Pins List",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("ae5f5169-aeb8-4127-b234-f28e3859a73a"),
                        ActivityParentId = Guid.Parse("17524c4b-119e-4b9f-ac30-734018a000d6"),
                        Permission = "upload-pins",
                        DisplayName = "Can Upload Pins",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("2a2f5d73-cf74-4067-a208-0518db8996af"),
                        ActivityParentId = Guid.Parse("17524c4b-119e-4b9f-ac30-734018a000d6"),
                        Permission = "used-pins",
                        DisplayName = "Used Pins List",
                        IsActive = true, Deleted = false},
                     

                      //app-settings
                    new AppActivity {
                        Id = Guid.Parse("473f9015-1472-4b22-aed1-8367c0e81091"),
                        ActivityParentId = Guid.Parse("65e70bfb-b760-41ae-86e1-14cde57dc21c"),
                        Permission = "portal-setting",
                        DisplayName = "Portal Setting",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("e5c29c00-b11b-4fb3-9c24-0e4c45ece807"),
                        ActivityParentId = Guid.Parse("65e70bfb-b760-41ae-86e1-14cde57dc21c"),
                        Permission = "template-setting",
                        DisplayName = "Template Setting",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("c2f7252e-a08a-454f-a22c-1a36eb854605"),
                        ActivityParentId = Guid.Parse("65e70bfb-b760-41ae-86e1-14cde57dc21c"),
                        Permission = "grade-setting",
                        DisplayName = "Grade Setting",
                        IsActive = true, Deleted = false},

                      //announcement
                    new AppActivity {
                        Id = Guid.Parse("4f70c13a-6dcb-428b-a204-c073da678f96"),
                        ActivityParentId = Guid.Parse("b63e6393-52ce-4ffc-8e9e-44a164c1030d"),
                        Permission = "announcement-list",
                        DisplayName = "Can See Announcements",
                        IsActive = true, Deleted = false},
                     new AppActivity {
                        Id = Guid.Parse("73886ef2-230c-4be7-9335-ab076d44877e"),
                        ActivityParentId = Guid.Parse("b63e6393-52ce-4ffc-8e9e-44a164c1030d"),
                        Permission = "create-announcement",
                        DisplayName = "Can Create Announcements",
                        IsActive = true, Deleted = false},
                      new AppActivity {
                        Id = Guid.Parse("81f8d684-63be-48f3-9e67-b307cf1337f1"),
                        ActivityParentId = Guid.Parse("b63e6393-52ce-4ffc-8e9e-44a164c1030d"),
                        Permission = "edit-announcement",
                        DisplayName = "Can Edit Announcements",
                        IsActive = true, Deleted = false},
                       new AppActivity {
                        Id = Guid.Parse("01950b80-7253-40c7-a12a-7c7f6ae64eb8"),
                        ActivityParentId = Guid.Parse("b63e6393-52ce-4ffc-8e9e-44a164c1030d"),
                        Permission = "delete-announcement",
                        DisplayName = "Can Delete Announcements",
                        IsActive = true, Deleted = false},


            };
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in activities)
                    {
                        if (!context.AppActivity.Any(w => w.Id == item.Id))
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
