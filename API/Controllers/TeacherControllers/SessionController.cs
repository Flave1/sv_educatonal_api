using BLL.Filter;
using BLL.MiddleWares;
using BLL.SessionServices;
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
            var response = await sessionService.CreateSessionAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllSessionsAsync(int pageNumber)
        {
            PaginationFilter filter = new PaginationFilter { PageNumber = pageNumber };
            var response = await sessionService.GetSessionsAsync(filter);
            return Ok(response);
        }

        [HttpGet("getall-single-session{sessionId}")]
        public async Task<IActionResult> GetSingleSessionAsync(string sessionId)
        {
            var response = await sessionService.GetSingleSessionAsync(sessionId);
            return Ok(response);
        }



        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSessionAsync([FromBody] SingleDelete reguest)
        {

            var response = await sessionService.DeleteSessionAsync(Guid.Parse(reguest.Item));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("switch-session")]
        public async Task<IActionResult> SwitchSessionAsync([FromBody] SwitchSession reguest)
        {
            var response = await sessionService.SwitchSessionAsync(reguest.SessionId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("activate-term")]
        public async Task<IActionResult> ActivateTermAsync([FromBody] ActivateTerm reguest)
        {
            var response = await sessionService.ActivateTermAsync(Guid.Parse(reguest.SessionTermId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-active")]
        public async Task<IActionResult> GetActiveSessionsAsync()
        {
            var response = await sessionService.GetActiveSessionsAsync();
            return Ok(response);
        }

        [HttpPost("update/header-teacher")]
        public async Task<IActionResult> UpdateSessionAsync([FromBody] UpdateHeadTeacher reguest)
        {

            var response = await sessionService.UpdateSessionHeadTeacherAsync(reguest);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-session-terms")]
        public async Task<IActionResult> GetSessionTermsAsync(string sessionId)
        {
            var response = await sessionService.GetSessionTermsAsync(Guid.Parse(sessionId));
            return Ok(response);
        }

        [HttpGet("get/session-term/classes")]
        public async Task<IActionResult> GetSessionTermClassesAsync(string sessionId)
        {
            var response = await sessionService.GetSessionClassesAsync(Guid.Parse(sessionId));
            return Ok(response);
        }

        #endregion



    }
}
