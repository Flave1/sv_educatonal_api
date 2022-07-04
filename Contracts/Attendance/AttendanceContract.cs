using DAL.Authentication;
using DAL.StudentInformation;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.AttendanceContract
{
    public class GetAttendance
    {
        public Guid ClassRegisterId { get; set; }
        public string ClassRegisterLabel { get; set; }
        public int TotalStudentInClass { get; set; }
        public int TotalStudentPresent{ get; set; }
        public int TotalStudentAbsent { get; set; }
        public List<AttendanceList> AttendanceList { get; set; }
        public GetAttendance(ClassRegister db, string regNoFormat)
        {
            ClassRegisterId = db.ClassRegisterId;
            ClassRegisterLabel = db.RegisterLabel;
            if (db.SessionClass.Students.Any())
                AttendanceList = db.SessionClass.Students.Select(d => new AttendanceList(d, regNoFormat, db.StudentAttendances)).ToList();
        }
        public GetAttendance(ClassRegister db)
        {
            ClassRegisterId = db.ClassRegisterId;
            ClassRegisterLabel = db.RegisterLabel;
            TotalStudentInClass = db.SessionClass.Students.Count();
            TotalStudentPresent = db.StudentAttendances.Count();
            TotalStudentAbsent = TotalStudentInClass - TotalStudentPresent;
        }
    }
    public class AttendanceList
    {
        public string StudentName { get; set; }
        public string RegistrationNumber { get; set; }
        public Guid StudentContactId { get; set; }
        public bool IsPresent { get; set; }
        public AttendanceList(StudentContact student, string regNoFormat, ICollection<StudentAttendance> studentsAttendance)
        {
            var stAtt = studentsAttendance.FirstOrDefault(d => d.StudentContactId == student.StudentContactId) ?? null;
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            StudentContactId = student.StudentContactId;
            IsPresent = stAtt == null ? false : true;
            StudentName = $"{ student.User.FirstName }" + $"{ student.User.LastName }";
        }
    }
    public class PostStudentAttendance
    {
        public Guid ClassRegisterId { get; set; } 
        public Guid StudentContactId { get; set; }
        public bool IsPresent { get; set; }
    } 
    public class GetPresentStudentAttendance
    {
        public long PresentStudent { get; set; } 
        public long AbsentStudent { get; set; }
    }

    public class GetClassRegister
    {
        public Guid ClassRegisterId { get; set; }
        public string RegisterLabel { get; set; }
    }
}
