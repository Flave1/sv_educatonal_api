using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PinManagementService;
using SMP.Contracts.PinManagement;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    //[PortalAuthorize]
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
        public async Task<IActionResult> UploadPin([FromForm] UploadPinRequest request)
        {
            var response = await service.UploadPinAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/uploadedpins")]
        public async Task<IActionResult> GetUploadedPin()
        {
            var response = await service.GetUploadedPinAsync();
            return Ok(response);
        }
        [HttpGet("get/usedpins")]
        public async Task<IActionResult> GetUsedPin()
        {
            var response = await service.GetUsedPinAsync();
            return Ok(response); 
        }
        [HttpGet("get/uploadedpin-detailed")]
        public async Task<IActionResult> GetUploadedPinDetails(string uploadedPinId)
        {
            var response = await service.GetUploadedPinDetailAsync(uploadedPinId);
            return Ok(response);
        }
        [HttpGet("get/usedpin-details")]
        public async Task<IActionResult> GetUsedPinDetails(string usedPinId)
        {
            var response = await service.GetUsedPinDetailedAsync(usedPinId);
            return Ok(response);
        }



        #endregion



    }
}
