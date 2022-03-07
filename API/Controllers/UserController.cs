using BLL.AuthenticationServices;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("user/api/v1/")]
    public class UserController : Controller
    { 
        private readonly IUserService userService;
        private readonly IStudentService studentService;
        private readonly IIdentityService identityService;
        public UserController(IUserService userService, IStudentService studentService, IIdentityService identityService)
        {
            this.userService = userService;
            this.studentService = studentService;
            this.identityService = identityService;
        }


        #region USERS

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        { 
            try
            {
                var result  = await identityService.LoginAsync(request);
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
        }

        [HttpPost("assign/user-to-role")]
        public async Task<IActionResult> AssignUserToRole([FromBody] AddToRole request)
        {

            try
            {
                await userService.AddUserToRoleAsync(request.RoleId, null, request.RoleId);
                return Ok(new { result = "Successfully added user to selected role" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpPost("create/teacher")]
        public async Task<IActionResult> CreateTeacherAsync([FromBody] UserCommand request)
        { 
            try
            {
                await userService.CreateTeacherAsync(request.Email);
                var result = await userService.GetAllTeachersAsync();
                return Ok(new { result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }
        [HttpPost("update/teacher")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateTeacher request)
        { 
            try
            {
                await userService.UpdateTeacherAsync(request);
                return Ok(new { result = true });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpGet("getall/teachers")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var result = await userService.GetAllTeachersAsync();
            return Ok(new { result = result });
        }
         

        [HttpPost("delete/teacher")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] SingleDelete reguest)
        {

            try
            {
                await userService.DeleteUserAsync(reguest.Item);
                return Ok(new { result = true });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [HttpPost("create/student")]
        public async Task<IActionResult> CreateTeacherAsync([FromBody] StudentContactCommand request)
        {
            try
            {
                await studentService.CreateStudenAsync(request); 
                return Ok(new { result = request });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }



        [HttpGet("getall/students")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            var result = await studentService.GetAllStudensAsync();
            return Ok(new { result = result });
        }

        [AllowAnonymous]
        [HttpPost("generate/reset-link")]
        public async Task<IActionResult> GenerateResetLinkAndSendToUserEmail([FromBody] ResetPassword request)
        {
            try
            {
                userService.ValidateResetOption(request);
                await userService.GenerateResetLinkAndSendToUserEmail(request);
                return Ok(new { result = "Please check your email to reset password" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset/password")]
        public async Task<IActionResult> ResetAccountAsync([FromBody] ResetAccount request)
        {
            try
            {
                await userService.ResetAccountAsync(request); 
                return Ok(new { result = "Password reset successful" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { result = ex.Message });
            }
        }

        #endregion




        public string token { get; set; }
        [HttpGet("/fetch/account")]
        public ActionResult GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;
            return Ok(userId);
        }
    }
}
