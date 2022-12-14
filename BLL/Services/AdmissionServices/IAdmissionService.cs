using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using SMP.Contracts.Admissions;
using SMP.Contracts.AdmissionSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AdmissionServices
{
    public interface IAdmissionService
    {
        Task<APIResponse<AdmissionLoginDetails>> Login(AdmissionLogin request);
        Task<APIResponse<bool>> ConfirmEmail(ConfirmEmail request);
        Task<APIResponse<bool>> DeleteEmail(SingleDelete request);
        Task<APIResponse<string>> CreateAdmission(CreateAdmission request);
        Task<APIResponse<PagedResponse<List<SelectAdmission>>>> GetAllAdmission(PaginationFilter filter);
        Task<APIResponse<SelectAdmission>> GetAdmission(string AdmissionId);
        Task<APIResponse<bool>> DeleteAdmission(SingleDelete request);
        Task<APIResponse<List<AdmissionClasses>>> GetAdmissionClasses();
    }
}
