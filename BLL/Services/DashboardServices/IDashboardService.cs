using BLL;
using SMP.Contracts.Dashboard;
using System.Collections.Generic;

namespace SMP.BLL.Services.DashboardServices
{
    public interface IDashboardService
    {
        APIResponse<List<ApplicationSetupStatus>> GetApplicationStatus();
        APIResponse<GetDashboardCount> GetDashboardCountAsync();
        APIResponse<GetStudentshDasboardCount> GetStudentDashboardCountAsync();
        APIResponse<List<Teacherclasses>> GetTeacherMobileDashboardCountAsync();
    }
}
