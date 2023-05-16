using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.MiddleWares;
using BLL.StudentServices;
using Contracts.Authentication;
using Contracts.Common;
using Contracts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMP.Contracts.Authentication;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{

    [PortalAuthorize]
    [Route("user/api/v1/")]
    public class UserController : Controller
    { 
        private readonly IUserService userService;
        private readonly IIdentityService identityService;
        public UserController(IUserService userService, IIdentityService identityService)
        {
            this.userService = userService;
            this.identityService = identityService;
        }


        #region USERS

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        { 
            try
            {
                var response  = await identityService.WebLoginAsync(request);
                if(response.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
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


      
        [AllowAnonymous]
        [HttpPost("first-time/change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePassword request)
        {
            try
            {
                var response = await userService.ChangePasswordAsync(request);
                if (response.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("validate/user-information/from-mobile")]
        public async Task<ActionResult<SmpStudentValidationResponse>> ValidateUserInformationFromMobileAsync([FromBody] UserInformationFromMobileRequest request)
        {
            try
            {
                var response = await userService.ValidateUserInformationFromMobileAsync(request);
                if (response.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
        }

        //public string token { get; set; }
        [HttpPost("get/mobile-permissions")]
        public async Task<ActionResult> GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;
            var response = await identityService.GetMobilePermissionsAsync(userId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("get/cbt-token")]
        public async Task<ActionResult> GetCBTTokenAsync()
        {
            var response = await identityService.GetCBTTokenAsync();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody]ForgotPassword request)
        {
            var response = await userService.ForgotPassword(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody]ResetAccount request)
        {
            var response = await userService.ResetPassword(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }




        [AllowAnonymous]
        [HttpPost("mobile-login")]
        public async Task<IActionResult> MobileLogin([FromBody] LoginCommand request)
        {
            try
            {
                var response = await identityService.MobileLoginAsync(request);
                if (response.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { result = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("forget/mobile-password")]
        public async Task<IActionResult> ForgotMobilePassword([FromBody] ValidateEmail request)
        {

            var response = await userService.ValidateEmailAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("validate-otp-mobile")]
        public async Task<IActionResult> ValidateOTPAsync([FromBody] ValidateOtp request)
        {

            var response = await userService.ValidateOTPAsync(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("reset-password-mobile")]
        public async Task<IActionResult> ResetPass([FromBody] ResetAccountMobile request)
        {
            var response = await userService.ResetPasswordMobile(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        #endregion



    }
}
