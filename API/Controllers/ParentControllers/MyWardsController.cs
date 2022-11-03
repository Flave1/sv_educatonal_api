using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ParentServices;
using System.Threading.Tasks;

namespace SMP.API.Controllers.ParentControllers
{
    [PortalAuthorize]
    [Route("smp/maywards/api/v1")]
    public class MyWardsController : Controller
    {
        private readonly IParentService service;
        public MyWardsController(IParentService service)
        {
            this.service = service;
        }

        [HttpGet("get/maywards")]
        public async Task<IActionResult> GetMyWardsAsync(int pageNumber)
        {
            var filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await service.GetMyWardsAsync(filter);
            return Ok(response);
        }


      
    }
}
