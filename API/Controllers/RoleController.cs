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
        private readonly IUserService userService;
        public RoleController(IRolesService service, IUserService userService)
        {
            this.roleService = service;
            this.userService = userService;
        }

        #region ROLES

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] ApplicationRoleCommand request)
        { 
            try
            {
                await roleService.CreateRoleAsync(request.Name);
                var result = await roleService.GetAllRolesAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            { 
                return StatusCode(400, new { result = ex.Message });
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] ApplicationRoleCommand request)
        { 
            try
            {
                await roleService.UpdateRoleAsync(request.Name, request.RoleId);
                var result = await roleService.GetAllRolesAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            { 
                return StatusCode(400, new { result = ex.Message });
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllRolesAsync()
        { 
            var result = await roleService.GetAllRolesAsync();
            return Ok(new { result = result });
        }

        [HttpGet("get/{roleId}")]
        public async Task<IActionResult> GetSingleRoleAsync(string roldeId)
        { 
            var result = await roleService.GetSingleRoleAsync(roldeId);
            return Ok(new { result = result });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody] MultipleDelete reguest)
        {
            
            try
            {
                if (reguest.Items.Any()) 
                    foreach(var item in reguest.Items)
                    { 
                        await roleService.DeleteRoleAsync(item);
                    } 
                return Ok(new { result = true});
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
        }

        #endregion



        [HttpPost("assign/user-to-role")]
        public async Task<IActionResult> AssignUserToRole([FromBody] AddToRole request)
        {

            try
            {
                await userService.AddUserToRoleAsync(request.RoleId, null, request.UserIds);
                return Ok(new { result = "Successfully added user to selected role" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }



    }
}
