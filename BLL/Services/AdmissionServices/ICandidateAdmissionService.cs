using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using SMP.Contracts.PortalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public interface ICandidateAdmissionService
    {
        Task<APIResponse<bool>> ConfirmEmail(ConfirmEmail request);
        Task<APIResponse<string>> CreateAdmission(CreateAdmission request);
        Task<APIResponse<PagedResponse<List<SelectCandidateAdmission>>>> GetAllAdmission(PaginationFilter filter, string admissionSettingsId);
        Task<APIResponse<SelectCandidateAdmission>> GetAdmission(string admissionId);
        Task<APIResponse<List<SelectAdmissionSettings>>> GetSettings();
        Task<APIResponse<bool>> DeleteAdmission(SingleDelete request);
        Task<APIResponse<List<AdmissionClasses>>> GetAdmissionClasses();
        Task<APIResponse<string>> UpdateAdmission(UpdateAdmission request);
        Task<APIResponse<SchoolSettingContract>> GetSchollSettingsAsync();
        Task<APIResponse<string>> RegisterParent(CreateAdmissionParent request);
    }
}
