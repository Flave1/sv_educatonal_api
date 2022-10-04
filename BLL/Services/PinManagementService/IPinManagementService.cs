using BLL;
using BLL.Filter;
using BLL.Wrappers;
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

        Task<APIResponse<PrintResult>> PrintResultAsync(PrintResultRequest request);
        Task<APIResponse<UploadPinRequest>> UploadPinAsync(UploadPinRequest request);
        Task<APIResponse<PagedResponse<List<GetPins>>>> GetAllUsedPinsAsync(string sessionId, string termId, PaginationFilter filter);
        Task<APIResponse<PagedResponse<List<GetPins>>>> GetAllUnusedPinsAsync(PaginationFilter filter); 
        Task<APIResponse<PinDetail>> GetUnusedPinDetailAsync(string pin);
        Task<APIResponse<PinDetail>> GetUsedPinDetailAsync(string pin);
        string GetStudentRealRegNumber(string regNo);
        Task<APIResponse<List<PrintResult>>> PrintBatchResultResultAsync(BatchPrintResultRequest2 request);
    }
}
