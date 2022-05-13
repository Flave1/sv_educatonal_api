using BLL;
using BLL.AuthenticationServices;
using BLL.MiddleWares;
using Contracts.Authentication;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("role/api/v1/")]
    public class RoleController : Controller
    {
        private readonly IRolesService roleService;  
        public RoleController(IRolesService service)
        {
            this.roleService = service; 
        }

        #region ROLES

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleActivity request)
        {
            var response = await roleService.CreateRoleAsync(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleActivity request)
        {
          var response = await roleService.UpdateRoleAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall-activities")]
        public async Task<IActionResult> GetAllActivitiesAsync()
        {
            var result = await roleService.GetAllActivitiesAsync();
            return Ok(result);
        }

        [HttpGet("getall-activities-by-roleId")]
        public async Task<IActionResult> GetActivitiesByRoleAsync(string roleId)
        {
            var result = await roleService.GetAllActivitiesAsync();
            return Ok(result);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllRolesAsync()
        { 
            var result = await roleService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("get/{roleId}")]
        public async Task<IActionResult> GetSingleRoleAsync(string roldeId)
        { 
            var result = await roleService.GetSingleRoleAsync(roldeId);
            return Ok(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody] MultipleDelete request)
        {
            var response = await roleService.DeleteRoleAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion
          

         
    }
}
