using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.AdmissionServices;
using SMP.Contracts.AdmissionSettings;
using System.Threading.Tasks;

namespace SMP.API.Controllers.AdmissionControllers
{
    [Route("smp/api/v1/candidate-admission")]
    public class CandidateAdmissionController: Controller
    {
        private readonly IAdmissionService service;

        public CandidateAdmissionController(IAdmissionService service)
        {
            this.service = service;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdmissionLogin request)
        {
            var response = await service.Login(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmEmail request)
        {
            var response = await service.ConfirmEmail(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
