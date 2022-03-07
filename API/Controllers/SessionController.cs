using BLL;
using BLL.AuthenticationServices;
using BLL.MiddleWares;
using BLL.SessionServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Session;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("session/api/v1/")]
    public class SessionController : Controller
    {
        private readonly ISessionService sessionService;  
        public SessionController(ISessionService service)
        {
            this.sessionService = service; 
        }

        #region Session

        [HttpPost("create")]
        public async Task<IActionResult> CreateSessionAsync([FromBody] CreateUpdateSession request)
        {
            
            try
            {
                await sessionService.CreateSessionAsync(request);
                var result = await sessionService.GetSessionsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateSessionAsync([FromBody] CreateUpdateSession request)
        {
            
            try
            {
                await sessionService.ModifySessionAsync(request);
                var result = await sessionService.GetSessionsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllSessionsAsync()
        { 
            var result = await sessionService.GetSessionsAsync();
            return Ok(new { result = result });
        }

       

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSessionAsync([FromBody] SingleDelete reguest)
        {
            
            try
            {
                await sessionService.DeleteSessionAsync(Guid.Parse(reguest.Item));
                return Ok(new { result = true});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpPost("switch")]
        public async Task<IActionResult> SwitchSessionAsync([FromBody] SwitchRequest reguest)
        { 
            try
            {
                await sessionService.SwitchSessionAsync(reguest.TargetId, reguest.SwitchValue);
                var response = reguest.SwitchValue ? $"Seccessfuly turned on Session" : $"Seccessfuly turned off Session";
                return Ok(new { result = response });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        #endregion



    }
}
