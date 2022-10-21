using BLL;
using SMP.Contracts.Dashboard;

namespace SMP.BLL.Services.DashboardServices
{
    public interface IDashboardService
    {
        APIResponse<GetDashboardCount> GetDashboardCountAsync();
        APIResponse<GetStudentshDasboardCount> GetStudentDashboardCountAsync();
    }
}
