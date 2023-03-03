using BLL;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMP.DAL.Models;

namespace API.Controllers.BaseControllers
{

    [Route("api/v1/base/")]
    public class BaseController : Controller
    {
        private readonly string smsClient;
        private readonly FwsClientInformation fwsClientInformations;
        public BaseController(IHttpContextAccessor accessor, FwsClientInformation fwsClientInformations)
        {
            smsClient = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.fwsClientInformations = fwsClientInformations;
        }
        //public static BaseService()
        //   : base()
        //{

        //}


        [HttpGet("is-authenticated/{clientId}")]
        [PortalAuthorize]
        public IActionResult IsStitllAuthenticated(string clientId)
        {
            var res = new APIResponse<bool>();
            res.IsSuccessful = true;
            res.Result = true;
            if (clientId != smsClient)
            {
                return Unauthorized(res);
            }
            return Ok(res);
        }

    }
}
