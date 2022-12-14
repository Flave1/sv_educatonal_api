using BLL;
using Contracts.Common;
using SMP.Contracts.Admission;
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
        Task<APIResponse<CreateAdmission>> CreateAdmission(CreateAdmission request);

    }
}
