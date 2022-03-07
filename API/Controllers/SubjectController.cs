using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BLL.Services.SubjectServices;
using Contracts.Class;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("subject/api/v1/")]
    public class SubjectController : Controller
    { 
        private readonly ISubjectService service; 
        public SubjectController(ISubjectService service)
        {
            this.service = service;
        }


        #region subject S

        [HttpPost("create/subject")]
        public async Task<IActionResult> CreateSubjectAsync([FromBody] ApplicationLookupCommand request)
        { 
            try
            {
                await service.CreateSubjectAsync(request.Name);
                var result = await service.GetAllSubjectsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }
        [HttpPost("update/subject")]
        public async Task<IActionResult> UpdateSubjectAsync([FromBody] ApplicationLookupCommand request)
        { 
            try
            {
                await service.UpdateSubjectAsync(request.Name, request.LookupId, request.IsActive);
                var result = await service.GetAllSubjectsAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("getall/subject")]
        public async Task<IActionResult> GetAllSubjectsAsync()
        {
            var result = await service.GetAllSubjectsAsync();
            return Ok(new { result = result });
        }
 

        [HttpPost("delete/subject")]
        public async Task<IActionResult> DeleteSubjectAsync([FromBody] SingleDelete reguest)
        { 
            try
            {
                await service.DeleteSubjectAsync(reguest.Item);
                var result = await service.GetAllSubjectsAsync();
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