using BLL;
using BLL.Filter;
using BLL.Wrappers;
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
        Task<APIResponse<PagedResponse<List<SelectAdmissionSettings>>>> GetAllSettings(PaginationFilter filter);
        Task<APIResponse<SelectAdmissionSettings>> GetSettingsById(string admissionSettingsId);
        Task<APIResponse<bool>> DeleteSettings(SingleDelete request);
        Task<APIResponse<UpdateAdmissionSettings>> UpdateSettings(UpdateAdmissionSettings request);
        Task<APIResponse<PagedResponse<List<SelectAdmissionSettings>>>> GetAllSettingsFromAdmissionScreen(PaginationFilter filter);
    }
}
