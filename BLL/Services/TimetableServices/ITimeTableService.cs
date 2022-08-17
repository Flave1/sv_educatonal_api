using BLL;
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
        Task<APIResponse<CreateClassTimeTable>> CreateClassTimeTableAsync(CreateClassTimeTable request);
        Task<APIResponse<CreateClassTimeTableTime>> CreateClassTimeTableTimeAsync(CreateClassTimeTableTime request);
        Task<APIResponse<CreateClassTimeTableTimeActivity>> CreateClassTimeTableTimeActivityAsync(CreateClassTimeTableTimeActivity request);
        Task<APIResponse<List<GetClassActivity>>> GetClassTimeTableAsync(Guid classId);
    }
}
