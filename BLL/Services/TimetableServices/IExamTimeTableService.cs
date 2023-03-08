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
    public interface IExamTimeTableService
    {
        Task<APIResponse<CreateExamTimeTableDay>> CreateExamTimeTableDayAsync(CreateExamTimeTableDay request);
        Task<APIResponse<CreateExamTimeTableTime>> CreateExamTimeTableTimeAsync(CreateExamTimeTableTime request);
        Task<APIResponse<UpdateExamTimeTableTimeActivity>> UpdateExamTimeTableTimeActivityAsync(UpdateExamTimeTableTimeActivity request);
        Task<APIResponse<List<GetActiveTimetableClasses>>> GetAllActiveClassesAsync();
        Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableAsync(Guid classId);
        Task<APIResponse<List<GetExamTimeActivityByDay>>> GetExamTimeActivityByDayAsync(string day);
        Task<APIResponse<SingleDelete>> DeleteExamTimeTableDayAsync(SingleDelete request);
        Task<APIResponse<SingleDelete>> DeleteExamTimeTableTimeAsync(SingleDelete request);
        Task<APIResponse<UpdateExamTimeTableTime>> UpdateExamTimeTableTimeAsync(UpdateExamTimeTableTime request);
        Task<APIResponse<UpdateExamTimeTableDay>> UpdateExamTimeTableDayAsync(UpdateExamTimeTableDay request);
        Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableByStudentAsync();
        Task<APIResponse<GetExamTimeActivity>> GetExamTimeTableByParentsAsync(Guid classlkpId);
    }
}
