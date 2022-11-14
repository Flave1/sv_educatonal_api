using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ParentModels
{
    public class MyWards
    {
        public string FullnaName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Class { get; set; }
        public string EnrollmentStatus { get; set; }
        public string Status { get; set; }
        public Guid ClassId { get; set; }
        public string StudentId { get; set; }
        public string ProfileUrl { get;set; }
        public MyWards(StudentContact x, string regNoFormat)
        {
            Class = x.SessionClass.Class.Name;
            ClassId = x.SessionClassId;
            StudentId = x.StudentContactId.ToString();
            FullnaName = x.User.FirstName + " " + x.User.LastName;
            ProfileUrl = x.User.Photo;
            RegistrationNumber = regNoFormat.Replace("%VALUE%", x.RegistrationNumber);
            if (x.Status == 1)
                Status = "Active";
            if (x.Status == 0)
                Status = "Inactive";
            if (x.EnrollmentStatus == 1)
                EnrollmentStatus = "Enrolled";
            if (x.EnrollmentStatus == 2 )
                EnrollmentStatus = "Not Enrolled";
        }
    }
}
