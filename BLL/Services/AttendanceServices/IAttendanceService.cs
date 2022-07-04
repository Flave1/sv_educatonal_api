using BLL; 
using Contracts.AttendanceContract;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<APIResponse<PostStudentAttendance>> UpdateStudentAttendanceRecord(PostStudentAttendance attendance);
        Task<APIResponse<List<GetAttendance>>> GetAllAttendanceRegisterAsync();
        Task<APIResponse<GetPresentStudentAttendance>> AbsentStudentAsync(PostStudentAttendance attendance);
        Task<APIResponse<ClassRegister>> CreateClassRegisterAsync(Guid SessionClassId);
        Task<APIResponse<GetPresentStudentAttendance>> PresentStudentAsync(PostStudentAttendance attendance);
        Task<APIResponse<List<GetAttendance>>> ContinueAttendanceAsync(Guid ClassRegisterId);
    }
}
