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
        public async Task<IActionResult> CreateTeacherAsync([FromBody] Promote request)
        {
            var response = await service.PromoteClassAsync(Guid.Parse(request.ClassToBePromoted), Guid.Parse(request.ClassToPromoteTo));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get/passed-students{sessionClassId}")]
        public async Task<IActionResult> GetPassedStudentsAsync(Guid sessioinClassId)
        {
            var response = await service.GetAllPassedStudentsAsync(sessioinClassId);
            return Ok(response);
        }

        [HttpGet("get/failed-students{sessionClassId}")]
        public async Task<IActionResult> GetFailedStudentsAsync(Guid sessioinClassId)
        {
            var response = await service.GetAllFailedStudentsAsync(sessioinClassId);
            return Ok(response);
        }

        #endregion



    }
}
