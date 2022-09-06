using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.PromorionServices;
using SMP.Contracts.PromotionModels;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("promotion/api/v1")]
    public class PromotionController : Controller
    {
        private readonly IPromotionService service;
        public PromotionController(IPromotionService sertvice)
        {
            this.service = sertvice;
        }


        #region PROMOTION

        [HttpGet("get/previous/session-classes")]
        public async Task<IActionResult> GetPreviousSessionClassesAsync()
        {
            var response = await service.GetPreviousSessionClassesAsync();
            return Ok(response);
        }

        [HttpPost("promote/class")]
        public async Task<IActionResult> PromoteClassAsync([FromBody] Promote request)
        {
            var response = await service.PromoteClassAsync(Guid.Parse(request.ClassToBePromoted), Guid.Parse(request.ClassToPromoteTo));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("get/passed-students")]
        public async Task<IActionResult> GetPassedStudentsAsync([FromBody] FetchPassedOrFailedStudents request)
        {
            var response = await service.GetAllPassedStudentsAsync(request);
            return Ok(response);
        }

        [HttpPost("get/failed-students")]
        public async Task<IActionResult> GetFailedStudentsAsync([FromBody]  FetchPassedOrFailedStudents request)
        {
            var response = await service.GetAllFailedStudentsAsync(request);
            return Ok(response);
        }

        #endregion



    }
}
