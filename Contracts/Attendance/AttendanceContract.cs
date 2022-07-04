using DAL.Authentication;
using SMP.DAL.Models.Attendance;
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
        public string StudentName { get; set; }
        public Guid StudentContactId { get; set; }
        public bool IsPresent { get; set; }
        public GetAttendance(Attendance db, AppUser user)
        {
            ClassRegisterId = db.ClassRegisterId;
            StudentContactId = db.StudentContactId;
            IsPresent = false;
            StudentName = $"{ user.FirstName }" + $"{ user.LastName }";
        }
    }
    public class PostAttendance
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
}
