﻿using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ParentModels
{
    public class GetParentWards
    {
        public string FullName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Class { get; set; }
        public string EnrollmentStatus { get; set; }
        public string Status { get; set; }
        public Guid ClassId { get; set; }
        public Guid ClassLkId { get; set; }
        public string StudentId { get; set; }
        public string ProfileUrl { get; set; }
        public GetParentWards(StudentContact x, string regNoFormat)
        {
            ClassLkId = x.SessionClass.ClassId;
            Class = x.SessionClass.Class.Name;
            ClassId = x.SessionClassId;
            StudentId = x.StudentContactId.ToString();
            FullName = x.FirstName + " " + x.LastName;
            ProfileUrl = x.Photo;
            RegistrationNumber = regNoFormat.Replace("%VALUE%", x.RegistrationNumber);
            if (x.Status == 1)
                Status = "Active";
            if (x.Status == 0)
                Status = "Inactive";
            if (x.EnrollmentStatus == 1)
                EnrollmentStatus = "Enrolled";
            if (x.EnrollmentStatus == 2)
                EnrollmentStatus = "Not Enrolled";
        }
    }
}
