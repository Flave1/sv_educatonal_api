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
        public async Task<IActionResult> UploadPinAsync([FromForm] UploadPinRequest request)
        {
            var response = await service.UploadPinAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("get/pins")]
        public async Task<IActionResult> GetAllPinsAsync()
        {
            var response = await service.GetAllPinsAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        #endregion



    }
}
