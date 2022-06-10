using DAL.ClassEntities;
using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PromotionModels
{
    public class PreviousSessionClasses
    {
        public string SessionClassId { get; set; }
        public string SessionClassName { get; set; }
        public int TotalStudentsInClass { get; set; }
        public int TotalStudentsPassed { get; set; }
        public int TotalStudentsFailed { get; set; }
        public PreviousSessionClasses(SessionClass cl)
        {
            SessionClassId = cl.SessionClassId.ToString();
            SessionClassName = cl.Class.Name;
            TotalStudentsInClass = cl.Students.Count();
            TotalStudentsPassed = cl.Students.Count();
            TotalStudentsFailed = 0;
        }

    }

    public class Promote
    {
        public string ClassToBePromoted { get; set; }
        public string ClassToPromoteTo { get; set; }

    }

    public class GetStudents
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SessionClassId { get; set; }
        public string Status { get; set; } 
        public string RegistrationNumber { get; set; }
        public GetStudents(StudentContact student, string status, string regNoFormat)
        {
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            StudentContactId = student.StudentContactId.ToString();
            StudentName = student.User.FirstName +" "+ student.User.LastName;
            ClassName = student.SessionClass.Class.Name;
            SessionClassId = student.SessionClass.SessionClassId.ToString();
            Status = status;
        }
    }
}
