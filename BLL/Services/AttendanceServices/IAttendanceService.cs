using BLL; 
using Contracts.AttendanceContract;
using SMP.DAL.Models.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<APIResponse<Attendance>> CreateAttendanceAsync(PostAttendance attendance); 
        Task<APIResponse<Attendance>> UpdateAttendanceAsync(PostAttendance attendance); 
        Task<APIResponse<List<GetAttendance>>> GetAllAttendanceRegisterAsync();
        Task<APIResponse<GetPresentStudentAttendance>> AbsentStudentAsync(PostAttendance attendance);
        Task<APIResponse<GetPresentStudentAttendance>> PresentStudentAsync(PostAttendance attendance); 
    }
}
