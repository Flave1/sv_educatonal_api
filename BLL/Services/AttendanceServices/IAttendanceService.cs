using BLL;
using Contracts.AttendanceContract;
using Contracts.Common;
using SMP.DAL.Models.Register;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<APIResponse<PostStudentAttendance>> UpdateStudentAttendanceRecord(PostStudentAttendance attendance);
        Task<APIResponse<List<GetAttendance>>> GetAllAttendanceRegisterAsync();
        Task<APIResponse<List<AttendanceList>>> GetAllAbsentStudents(Guid classRegisterId);
        Task<APIResponse<ClassRegister>> CreateClassRegisterAsync(Guid SessionClassId);
        Task<APIResponse<List<AttendanceList>>> GetAllStudentPresentAsync(Guid classRegisterId);
        Task<APIResponse<List<GetAttendance>>> ContinueAttendanceAsync(Guid ClassRegisterId);
        Task<APIResponse<bool>> DeleteClassRegisterAsync(SingleDelete ClassRegister);
        Task<APIResponse<UpdateClassRegister>> UpdateClassRegisterLabel(UpdateClassRegister ClassRegister);
    }
}
