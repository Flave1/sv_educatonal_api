using BLL;
using Contracts.Class;
using Contracts.Common;
using SMP.Contracts.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TimetableServices
{
    public interface ITimeTableService
    {
        Task<APIResponse<CreateClassTimeTableDay>> CreateClassTimeTableDayAsync(CreateClassTimeTableDay request);
        Task<APIResponse<CreateClassTimeTableTime>> CreateClassTimeTableTimeAsync(CreateClassTimeTableTime request);
        Task<APIResponse<UpdateClassTimeTableTimeActivity>> UpdateClassTimeTableTimeActivityAsync(UpdateClassTimeTableTimeActivity request);
        Task<APIResponse<List<GetApplicationLookups>>> GetAllActiveClassesAsync();
        Task<APIResponse<GetClassTimeActivity>> GetClassTimeTableAsync(Guid classId);
        Task<APIResponse<List<GetClassTimeActivityByDay>>> GetClassTimeActivityByDayAsync(string day);
        Task<APIResponse<SingleDelete>> DeleteClassTimeTableDayAsync(SingleDelete request);
        Task<APIResponse<SingleDelete>> DeleteClassTimeTableTimeAsync(SingleDelete request);
        Task<APIResponse<UpdateClassTimeTableTime>> UpdateClassTimeTableTimeAsync(UpdateClassTimeTableTime request);
        Task<APIResponse<UpdateClassTimeTableDay>> UpdateClassTimeTableDayAsync(UpdateClassTimeTableDay request);
        Task<APIResponse<GetClassTimeActivity>> GetClassTimeTableByStudentAsync();
    }
}
