using BLL;
using Contracts.Common;
using SMP.Contracts.AdmissionSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public interface IAdmissionSettingService
    {
        Task<APIResponse<CreateAdmissionSettings>> CreateSettings(CreateAdmissionSettings request);
        Task<APIResponse<SelectAdmissionSettings>> GetSettings();
        Task<APIResponse<bool>> DeleteSettings(SingleDelete request);
    }
}
