using BLL.Constants;
using BLL.LoggerService;
using Contracts.Authentication;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMP.BLL.Constants;
using SMP.BLL.Services;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.PinManagement;
using SMP.Contracts.Routes;
using SMP.DAL.Models.PortalSettings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public class IdentityService : IIdentityService
    {


        public IdentityService(UserManager<AppUser> userManager, TokenValidationParameters tokenValidationParameters,
            RoleManager<UserRole> roleManager, ILoggerService logger, IOptions<JwtSettings> jwtSettings,
            DataContext context, IHttpContextAccessor accessor, IWebRequestService webRequestService, IOptions<FwsConfigSettings> options)
        {
            this.userManager = userManager;
            this.tokenValidationParameters = tokenValidationParameters;
            this.roleManager = roleManager;
            this.logger = logger;
            this.jwtSettings = jwtSettings.Value;
            this.context = context;
            this.accessor = accessor;
            this.webRequestService = webRequestService;
            fwsOptions = options.Value;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly JwtSettings jwtSettings;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly RoleManager<UserRole> roleManager;
        private readonly ILoggerService logger;
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IWebRequestService webRequestService;
        private readonly FwsConfigSettings fwsOptions;

        async Task<APIResponse<LoginSuccessResponse>> IIdentityService.WebLoginAsync(LoginCommand loginRequest)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            res.Result = new LoginSuccessResponse();
            try
            {
                var id = Guid.NewGuid();
                var userAccount = await userManager.FindByNameAsync(loginRequest.UserName);
                var permisions = new List<string>();
                if (userAccount == null)
                {
                    res.Message.FriendlyMessage = $"User account with {loginRequest.UserName} is not available";
                    return res;
                }

                if (!await userManager.CheckPasswordAsync(userAccount, loginRequest.Password))
                {
                    res.Message.FriendlyMessage = $"Password seems to be incorrect";
                    return res;
                }

                if(userAccount.UserType == (int)UserTypes.Admin)
                {
                    permisions = context.AppActivity.Where(d => d.IsActive).Select(s => s.Permission).OrderBy(s => s).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Teacher)
                {
                    var techerAccount = await context.Teacher.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (techerAccount != null && techerAccount.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = techerAccount.TeacherId;

                    var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                    permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive & userRoleIds.Contains(d.RoleId)).Select(s => s.Activity.Permission).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Student)
                {
                    var studentAccount = await context.StudentContact.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (studentAccount != null && studentAccount.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = studentAccount?.StudentContactId ?? new Guid();
                }
                AppLayoutSetting appSettings = null;

                if (userAccount.UserType == (int)UserTypes.Parent)
                {
                }

                if (!string.IsNullOrEmpty(loginRequest.SchoolUrl))
                    appSettings = await context.AppLayoutSetting.FirstOrDefaultAsync(x => x.schoolUrl.ToLower() == loginRequest.SchoolUrl.ToLower());

                var schoolSetting = context.SchoolSettings.FirstOrDefault(x => x.ClientId == appSettings.ClientId) ?? new SchoolSetting();

                res.Result = new LoginSuccessResponse();
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id, permisions, appSettings);
                res.Result.UserDetail = new UserDetail(schoolSetting, userAccount, id);
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = "Unexpected Error Occurred";
                return res;
            }
        }

        async Task<APIResponse<MobileLoginSuccessResponse>> IIdentityService.MobileLoginAsync(LoginCommand loginRequest)
        {
            var res = new APIResponse<MobileLoginSuccessResponse>();
            res.Result = new MobileLoginSuccessResponse();
            try
            {
                var id = Guid.NewGuid();
                var userAccount = await userManager.FindByNameAsync(loginRequest.UserName);
                if (userAccount == null)
                {
                    res.Message.FriendlyMessage = $"User account with {loginRequest.UserName} is not available";
                    return res;
                }

                if (!await userManager.CheckPasswordAsync(userAccount, loginRequest.Password))
                {
                    res.Message.FriendlyMessage = $"Password seems to be incorrect";
                    return res;
                }

                

                if (userAccount.UserType == (int)UserTypes.Teacher)
                {
                    var techerAccount = await context.Teacher.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (techerAccount != null && techerAccount.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = techerAccount.TeacherId;

                }

                if (userAccount.UserType == (int)UserTypes.Student)
                {
                    var studentAccount = await context.StudentContact.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (studentAccount != null && studentAccount.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = studentAccount?.StudentContactId ?? new Guid();
                }


                res.Result = new MobileLoginSuccessResponse();
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id);
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<List<string>>> IIdentityService.GetMobilePermissionsAsync(string userId)
        {
            var res = new APIResponse<List<string>>();
            res.Result = new List<string>();
            try
            {
                var id = Guid.NewGuid();
                var userAccount = await userManager.FindByIdAsync(userId);
                var permisions = new List<string>();
                if (userAccount == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                if (userAccount.UserType == (int)UserTypes.Admin)
                {
                    permisions = context.AppActivity.Where(d => d.IsActive).Select(s => s.Permission).OrderBy(s => s).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Teacher)
                {
                    var techerAccount = await context.Teacher.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (techerAccount != null && techerAccount.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = techerAccount.TeacherId;

                    var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                    permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive 
                    & d.Deleted == false
                    & userRoleIds.Contains(d.RoleId)).Select(s => s.Activity.Permission).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Student)
                {
                    var studentAccount = await context.StudentContact.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (studentAccount != null && studentAccount.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                }

                res.Result = permisions;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string token)
        { 
            try
            {
                var validatedToken = GetClaimsPrincipalFromToken(token);
                if (validatedToken == null) 
                    throw new ArgumentException("Invalid Token"); 


                var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                     .AddDays(expiryDateUnix);

                if (expiryDateTimeUtc > DateTime.UtcNow)
                    throw new ArgumentException("This Token Hasn't Expired Yet"); 


                var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
                var storedRefreshToken = context.RefreshToken.SingleOrDefault(x => x.Token == refreshToken);

                if (storedRefreshToken == null)
                    throw new ArgumentException("This Token Hasn't Expired Yet");

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                    throw new ArgumentException("This Refresh Token has Expire"); 

                if (storedRefreshToken.Invalidated)
                    throw new ArgumentException("This Refresh Token has Expire");

                if (storedRefreshToken.Used)
                    throw new ArgumentException("This Refresh Token has Expire"); 

                if (storedRefreshToken.JwtId != jti)
                    throw new ArgumentException("This Refresh Token has been Used"); 


                storedRefreshToken.Used = true;
                context.RefreshToken.Update(storedRefreshToken);
                await context.SaveChangesAsync();

                var user = await userManager.FindByIdAsync(validatedToken.Claims.SingleOrDefault(x => x.Type == "userId").Value);

                return await GenerateAuthenticationResultForUserAsync(user, new Guid());
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error Occurred !! Please contact system administration"); 
            }
        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;
                else
                    return principal;
            }
            catch (Exception ex)
            {
                logger.Error($"Ex: {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase);
        }



        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(AppUser user, Guid ID, List<string> permissions = null, AppLayoutSetting appLayoutSetting = null)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("userId", user.Id),
                    new Claim("userType", user.UserType.ToString()),
                    new Claim("userName",user.UserName),
                    new Claim("name",user.FirstName + " " + user.LastName),
                    permissions != null ? new Claim("permissions", string.Join(',', permissions)) : new Claim("permissions", string.Join(',', "N/A")),
                    user.UserType == (int)UserTypes.Teacher ? new Claim("teacherId", ID.ToString()) :  new Claim("studentContactId", ID.ToString()),
                    appLayoutSetting != null ? new Claim("smsClientId", appLayoutSetting.ClientId) : new Claim("smsClientId",user.ClientId),
                };
                accessor.HttpContext.Items["smsClientId"] = user.ClientId;

                var userClaims = await userManager.GetClaimsAsync(user);

                claims.AddRange(userClaims);

                var userRoles = await userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole) ?? null);

                    var role = await roleManager.FindByNameAsync(userRole);

                    if (role == null)
                        continue;
                    var roleClaims = await roleManager.GetClaimsAsync(role);

                    foreach (var roleClaim in roleClaims)
                    {
                        if (claims.Contains(roleClaim)) continue;
                        claims.Add(roleClaim);
                    }
                }
                var tokenDecriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(jwtSettings.TokenLifeSpan),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                 
                var token = tokenHandler.CreateToken(tokenDecriptor);

                var refreshToken = new RefreshToken
                { 
                    JwtId = token.Id,
                    UserId = user.Id,
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddSeconds(6),
                };

                try
                {
                    await context.RefreshToken.AddAsync(refreshToken);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Something went wrong: {ex.InnerException.Message}");
                }

                return new AuthenticationResult
                {
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.JwtId,
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Something went wrong: {ex.InnerException.Message}");
            }

        }


        public async Task<AppUser> FetchLoggedInUserDetailsAsync(string userId)
        {
            var currentUser = await userManager?.FindByIdAsync(userId);
            return currentUser; 
        }

        async Task<APIResponse<LoginSuccessResponse>> IIdentityService.LoginAfterPasswordIsChangedAsync(AppUser userAccount, string schoolUrl)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            res.Result = new LoginSuccessResponse();
            try
            {
                var id = Guid.NewGuid();
                var permisions = new List<string>();
                AppLayoutSetting appSettings = null;

                if (!string.IsNullOrEmpty(schoolUrl))
                    appSettings = await context.AppLayoutSetting.FirstOrDefaultAsync(x => x.schoolUrl.ToLower() == schoolUrl.ToLower());
                else
                {
                    res.Message.FriendlyMessage = $"Invalid request on login";
                    return res;
                }

                if (userAccount.UserType == (int)UserTypes.Admin)
                {
                    var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                    permisions = context.AppActivity.Where(d => d.IsActive && d.ClientId == appSettings.ClientId).Select(s => s.Permission).OrderBy(s => s).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Teacher)
                {
                    var techerAccount = await context.Teacher.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (techerAccount != null && techerAccount.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = techerAccount.TeacherId;

                    var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                    permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive & userRoleIds.Contains(d.RoleId) && d.ClientId == appSettings.ClientId).Select(s => s.Activity.Permission).Distinct().ToList();
                }

                if (userAccount.UserType == (int)UserTypes.Student)
                {
                    var studentAccount = await context.StudentContact.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                    if (studentAccount != null && studentAccount.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = studentAccount?.StudentContactId ?? new Guid();
                }

                

                var schoolSetting = context.SchoolSettings.FirstOrDefault(x => x.ClientId == appSettings.ClientId) ?? new SchoolSetting();

                res.Result = new LoginSuccessResponse();
                userAccount.EmailConfirmed = true;
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id, permisions);
                res.Result.UserDetail = new UserDetail(schoolSetting, userAccount, id);
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task<APIResponse<CBTLoginDetails>> IIdentityService.GetCBTTokenAsync()
        {

            var res = new APIResponse<CBTLoginDetails>();
            var apiCredentials = new SmsClientInformationRequest
            {
                ApiKey = fwsOptions.Apikey,
                ClientId = fwsOptions.ClientId
            };
            var fwsClientInformation = await webRequestService.PostAsync<SmsClientInformation, SmsClientInformationRequest>($"{fwsRoutes.clientInformation}clientId={fwsOptions.ClientId}&apiKey={fwsOptions.Apikey}", apiCredentials);

            var clientDetails = new Dictionary<string, string>();
            clientDetails.Add("userId", fwsClientInformation.Result.UserId);
            clientDetails.Add("smsClientId", "");
            clientDetails.Add("productBaseurlSuffix", fwsClientInformation.Result.BaseUrlAppendix);

            var req = new LoginCommandByHash
            {
                PasswordHash = fwsClientInformation.Result.PasswordHash
            };

            res = await webRequestService.PostAsync<APIResponse<CBTLoginDetails>, LoginCommandByHash>($"{cbtRoutes.getToken}", req, clientDetails);
            if (res.Result == null)
            {
                res.Message.FriendlyMessage = res.Message.FriendlyMessage;
                return res;
            }
            res.IsSuccessful = true;
            return res;

        }
    }
}

