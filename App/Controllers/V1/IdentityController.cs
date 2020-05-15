using System;
using System.Linq;
using System.Threading.Tasks;
using App.LogHandler.Service;
using App.Contracts.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Contracts.Response;
using App.Contracts.Requests.Auth;
using App.AuthHandler.Interface;

namespace Libraryhub.Controllers.V1
{

    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly ILoggerService _loggerService;

        public IdentityController(IIdentityService identityService, ILoggerService loggerService)
        { 
            _identityService = identityService;
            _loggerService = loggerService;
        }
        
        [HttpPost(ApiRoutes.Identity.REGISTER)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationReqObj regRequest)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(regRequest);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }


            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.LOGIN)]
        public async Task<IActionResult> Login([FromBody] UserLoginReqObj request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var authResponse = await _identityService.LoginAsync(request.UserName, request.Password);
            if (!authResponse.Success && !authResponse.IsFirstLogin)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.REFRESHTOKEN)]
        public async Task<IActionResult> Refresh([FromBody] UserRefreshTokenReqObj request)
        {

            var authResponse = await _identityService.RefreshTokenAsync(request.RefreshToken, request.Token);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

       
        [HttpPost(ApiRoutes.Identity.CHANGE_PASSWORD)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword request)
        {

            if (request.Email.Length < 1)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Email Required" }
                });
            }

            if (request.OldPassword.Length < 1)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Old Password Required" }
                });
            }

            if (request.NewPassword.Length < 1)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "New Password Required" }
                });
            }


            var authResponse = await _identityService.ChangePasswsord(request);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.CONFIRM_EMAIL)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirnmationRequest request)
        {

            try
            {
                if (request.Email.Length < 1)
                {
                    return BadRequest(new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Email Required change password"
                            }

                        }
                    });
                }


                var userExist = await _identityService.CheckUserAsync(request.Email);
                if (!userExist)
                {

                    return BadRequest(new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Email not found"
                            }

                        }
                    });
                }
                var response = await _identityService.ConfirmEmailAsync(request);
                if (!response.Status.IsSuccessful)
                {
                    return BadRequest(new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = response.Status.Message.FriendlyMessage
                            }

                        }
                    });
                }
                return Ok(new ConfirnmationResponse
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true
                    }
                });
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.Message ?? ex.InnerException.Message);
                return BadRequest(ex.Message ?? ex.InnerException.Message);
            }
            
        }

        [HttpPost(ApiRoutes.Identity.CONFIRM_CODE)]
        public async Task<IActionResult> ConfirmationCode([FromBody] ConfirnmationRequest request)
        {

            if (request.Code.Length < 4)
            {
                return BadRequest(new ConfirnmationResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Invalid Verification Code"
                        }

                    }
                });
            } 

            var userExist = await _identityService.VerifyAsync(request.Email);
            if (!userExist.Status.IsSuccessful)
            {

                return BadRequest(new ConfirnmationResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = userExist.Status.Message.FriendlyMessage
                        } 
                    }
                });
            } 
            return Ok(new ConfirnmationResponse
            {

                Status = new APIResponseStatus
                {
                    IsSuccessful = true
                }
            });
        }

        public string token { get; set; }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Identity.FETCH_USERDETAILS)]
        public async Task<ActionResult<UserDataResponseObj>> GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;

            var profile = await _identityService.FetchLoggedInUserDetailsAsync(userId);
            if (!profile.Status.IsSuccessful)
            {
                return BadRequest(profile.Status);
            }
            return Ok(profile);
        }


         

    }
}