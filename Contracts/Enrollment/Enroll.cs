using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Enrollment
{
    public  class Enroll
    {
        public string SessionClassId { get; set; }
        public string[] StudentContactIds { get; set; }
    }
    public class UnEnroll
    {
        public string[] StudentContactIds { get; set; }
    }

    public class EnrolledStudents
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string StudentRegNumber { get; set; }
        public string Status { get; set; }
        public string Class { get; set; }
        public string SessionClassId { get; set; }
        public EnrolledStudents() { }
        public EnrolledStudents(StudentContact a, string regNoFormat)
        {
            Status = "enrrolled";
            StudentContactId = a.StudentContactId.ToString();
            StudentName = a.FirstName + " " + a.MiddleName + " " + a.LastName;
            StudentRegNumber = regNoFormat.Replace("%VALUE%", a.RegistrationNumber);
            Class = a.SessionClass.Class.Name;
            SessionClassId = a.SessionClassId.ToString();
        }
    }
}
