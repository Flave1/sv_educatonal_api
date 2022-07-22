using BLL;
using SMP.Contracts.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.DashboardServices
{
    public interface IDashboardService
    {
        Task<APIResponse<GetDashboard>> GetDashboardCountAsync();
    }
}
