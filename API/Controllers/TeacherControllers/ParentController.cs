using BLL.Filter;
using BLL.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ParentServices;
using SMP.DAL.Migrations;
using System.Threading.Tasks;

namespace SMP.API.Controllers.TeacherControllers
{
    [PortalAuthorize]
    [Route("parent/api/v1")]
    public class ParentController: Controller
    {
        private readonly IParentService service;
        public ParentController(IParentService service)
        {
            this.service = service;
        }
        [HttpGet("get-all/parents")]
        public async Task<IActionResult> GetParentsAsync(PaginationFilter filter)
        {
            var response = await service.GetParentsAsync(filter);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/parent/{parentId}")]
        public async Task<IActionResult> GetParentByIdAsync(string parentId)
        {
            var response = await service.GetParentByIdAsync(parentId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get/parent-wards")]
        public async Task<IActionResult> GetParentWardsAsync(PaginationFilter filter, string parentId)
        {
            var response = await service.GetParentWardsAsync(filter, parentId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
