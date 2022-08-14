﻿using BLL.AuthenticationServices;
using BLL.Constants;
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
                var response  = await identityService.LoginAsync(request);
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

        #endregion




        public string token { get; set; }
        [HttpGet("/fetch/account")]
        public ActionResult GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;
            string userType = HttpContext.User?.FindFirst(c => c.Type == "userType").Value;
            if((int)UserTypes.Teacher == Convert.ToInt16(userType))
            {

            }
            else
            {

            }
            return Ok(userId);
        }
    }
}