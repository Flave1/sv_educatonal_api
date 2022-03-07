using BLL.ClassServices;
using BLL.MiddleWares;
using Contracts.Class;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("class/api/v1/")]
    public class ClassController : Controller
    { 
        private readonly IClassService service;
        private readonly IClassLookupService lookupService;
        public ClassController(IClassService service, IClassLookupService lookupService)
        {
            this.lookupService = lookupService;
            this.service = service;
        }


        #region CLASS LOOKUPS

        [HttpPost("create/class-lookup")]
        public async Task<IActionResult> CreateClassLookupAsync([FromBody] ApplicationLookupCommand request)
        { 
            try
            {
                await lookupService.CreateClassLookupAsync(request.Name);
                var result = await lookupService.GetAllClassLookupsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }
        [HttpPost("update/class-lookup")]
        public async Task<IActionResult> UpdateClassLookupAsync([FromBody] ApplicationLookupCommand request)
        { 
            try
            {
                await lookupService.UpdateClassLookupAsync(request.Name, request.LookupId, request.IsActive);
                var result = await lookupService.GetAllClassLookupsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("getall/class-lookup")]
        public async Task<IActionResult> GetAllClassLookupsAsync()
        {
            var result = await lookupService.GetAllClassLookupsAsync();
            return Ok(new { result = result });
        }
 

        [HttpPost("delete/class-lookup")]
        public async Task<IActionResult> DeleteClassLookupAsync([FromBody] SingleDelete reguest)
        { 
            try
            {
                await lookupService.DeleteClassLookupAsync(reguest.Item);
                var result = await lookupService.GetAllClassLookupsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        #endregion

        #region CLASS

        [HttpPost("create/class")]
        public async Task<IActionResult> CreateClassAsync([FromBody] SessionClassCommand request)
        {
            try
            {
                await service.CreateSessionClassAsync(request);
                var result = await service.GetSessionClassesAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("get-all/classes")]
        public async Task<IActionResult> GetClassesAsync()
        {
            try
            { 
                var result = await service.GetSessionClassesAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("search/classes/by-session")]
        public async Task<IActionResult> GetClassesBySessionAsync([FromBody] SessionQuery query)
        {
            try
            {
                var result = await service.GetSessionClassesBySessionAsync(query.StartDate, query.EndDate);
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }



        #endregion


    }
} 