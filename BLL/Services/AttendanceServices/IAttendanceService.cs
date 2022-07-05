using BLL; 
using Contracts.AttendanceContract;
using DAL.StudentInformation;
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
        Task<APIResponse<List<StudentContact>>> AbsentStudentAsync(Guid classRegisterId);
        Task<APIResponse<ClassRegister>> CreateClassRegisterAsync(Guid SessionClassId);
        Task<APIResponse<List<StudentContact>>> PresentStudentAsync(Guid classRegisterId);
        Task<APIResponse<List<GetAttendance>>> ContinueAttendanceAsync(Guid ClassRegisterId);
        Task<APIResponse<DeleteClassRegisterContract>> DeleteClassRegisterAsync(DeleteClassRegisterContract ClassRegister);
        Task<APIResponse<UpdateClassRegisterContract>> UpdateClassRegisterLabel(UpdateClassRegisterContract ClassRegister);
    }
}
