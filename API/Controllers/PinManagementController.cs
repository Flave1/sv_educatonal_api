using BLL.MiddleWares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Helper;
using SMP.BLL.Helper.Model;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
     
    [Route("pinmanagement/api/v1/")]
    public class PinManagementController : Controller
    {
        private readonly IClientService service;
        public PinManagementController(IClientService service)
        {
            this.service = service;
        }


        [HttpGet("getby")]
        public async Task<IActionResult> GetBy([FromQuery] GetSms request)
        {
            var response = await service.GetBy(request);
            return Ok(response);
        }
    }
}
