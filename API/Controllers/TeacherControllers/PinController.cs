using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PinManagementService;
using SMP.Contracts.PinManagement;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("pin/api/v1")]
    public class PinController : Controller
    {
        private readonly IPinManagementService service;
        public PinController(IPinManagementService sertvice)
        {
            this.service = sertvice;
        }


        #region Pin


        [HttpPost("upload/pin")]
        public async Task<IActionResult> UploadPinAsync([FromForm] UploadPinRequest request)
        {
            var response = await service.UploadPinAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/unused-pins")]
        public async Task<IActionResult> GetAllUnusedPinsAsync(PaginationFilter filter)
        {
            var response = await service.GetAllUnusedPinsAsync(filter);
            return Ok(response);
        }
        [HttpGet("get/used-pins")]
        public async Task<IActionResult> GetAllUsedPinsAsync(string sessionId, string termId, int pageSize)
        {
            var filter = new PaginationFilter { PageSize = pageSize };
            var response = await service.GetAllUsedPinsAsync(sessionId, termId, filter);
            return Ok(response); 
        }
        [HttpGet("get-unused/pin-details")]
        public async Task<IActionResult> GetUnusedPinDetailAsync(string pin)
        {
            var response = await service.GetUnusedPinDetailAsync(pin);
            return Ok(response);
        }
        [HttpGet("get-used/pin-details")]
        public async Task<IActionResult> GetUsedPinDetails(string pin)
        {
            var response = await service.GetUsedPinDetailAsync(pin);
            return Ok(response);
        }



        #endregion



    }
}
