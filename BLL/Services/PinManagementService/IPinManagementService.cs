using BLL;
using SMP.Contracts.PinManagement;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PinManagementService
{
    public interface IPinManagementService
    {

        Task<APIResponse<PreviewResult>> PrintResultAsync(PrintResultRequest request);
        Task<APIResponse<UploadPinRequest>> UploadPinAsync(UploadPinRequest request);
        Task<APIResponse<List<GetUsedPinRequest>>> GetUsedPinAsync();
        Task<APIResponse<List<GetUploadPinRequest>>> GetUploadedPinAsync(); 
        Task<APIResponse<GetUsedPinRequest>> GetUsedPinDetailedAsync(string usedPinId, string SessionTermId);
        Task<APIResponse<GetUploadPinRequest>> GetUploadedPinDetailAsync(string uploadedPinId, string sessionTermId);
    }
}
