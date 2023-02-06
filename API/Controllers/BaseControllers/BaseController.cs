using BLL;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.BaseControllers
{

    [Route("api/v1/base/")]
    public class BaseController : Controller
    {
        public BaseController() { }


        [HttpGet("is-authenticated")]
        [PortalAuthorize]
        public IActionResult IsStitllAuthenticated()
        {
            var res = new APIResponse<bool>();
            res.IsSuccessful = true;
            res.Result = true;
            return Ok(res);
        }

    }
}
