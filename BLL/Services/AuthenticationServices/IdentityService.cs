using BLL.Constants;
using BLL.LoggerService;
using Contracts.Authentication;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMP.BLL.Constants;
using SMP.BLL.Services.WebRequestServices;
using SMP.BLL.Utilities;
using SMP.Contracts.Authentication;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using SMP.DAL.Models.Parents;
using SMP.DAL.Models.PortalSettings;
using SMP.DAL.Models.StudentImformation;
using SMP.DAL.Models.TeachersInfor;
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
            RoleManager<UserRole> roleManager, ILoggerService loggerService, IOptions<JwtSettings> jwtSettings,
            DataContext context, IHttpContextAccessor accessor, IWebRequestService webRequestService, IOptions<FwsConfigSettings> options, IUtilitiesService utilitiesService)
        {
            this.userManager = userManager;
            this.tokenValidationParameters = tokenValidationParameters;
            this.roleManager = roleManager;
            this.jwtSettings = jwtSettings.Value;
            this.context = context;
            this.accessor = accessor;
            this.webRequestService = webRequestService;
            this.loggerService = loggerService;
            fwsOptions = options.Value;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.utilitiesService = utilitiesService;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly JwtSettings jwtSettings;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly RoleManager<UserRole> roleManager;
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IWebRequestService webRequestService;
        private readonly ILoggerService loggerService;
        private readonly FwsConfigSettings fwsOptions;
        private readonly IUtilitiesService utilitiesService;
        private static string smsClientId { get; set; }


        async Task<APIResponse<LoginSuccessResponse>> IIdentityService.WebLoginAsync(LoginCommand loginRequest)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            res.Result = new LoginSuccessResponse();
            string firstName = string.Empty;
            string lastName = string.Empty;
            string userType = string.Empty;
            try
            {
                var id = Guid.NewGuid();
                var clientId = ClientId(loginRequest.SchoolUrl);
                var permisions = new List<string>();

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

                

                if (loginRequest.UserType == (int)UserTypes.Teacher)
                {
                    var teacher = GetTeacherByUserId(userAccount.Id, clientId);
                    if (teacher is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }

                    if (teacher.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }

                    if (utilitiesService.IsThisUser(UserTypes.Admin, userAccount.UserTypes))
                    {
                        permisions = context.AppActivity
                            .Where(d => d.IsActive).Select(s => s.Permission).Distinct().OrderBy(s => s).Distinct().ToList();
                        userType = UserTypes.Admin.ToString();
                    }
                    else
                    {
                        var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                        permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive & userRoleIds.Contains(d.RoleId) && d.ClientId == clientId).Select(s => s.Activity.Permission).Distinct().ToList();
                        userType = UserTypes.Teacher.ToString();
                    }
                    id = teacher.TeacherId;
                    firstName = teacher.FirstName;
                    lastName = teacher.LastName;
                }

                if (loginRequest.UserType == (int)UserTypes.Student)
                {
                    var student = GetStudentByUserId(userAccount.Id, clientId);
                    if (student is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }
                    if (student.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = student?.StudentContactId ?? new Guid();
                    firstName = student.FirstName;
                    lastName = student.LastName;
                    userType = UserTypes.Student.ToString();
                }
                SchoolSetting schoolSettings = null;

                if (loginRequest.UserType == (int)UserTypes.Parent)
                {
                    var parent = GetParentByUserId(userAccount.Id, clientId);
                    if (parent is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }
                    firstName = parent.FirstName;
                    lastName = parent.LastName;
                    id = parent.Parentid;
                    userType = UserTypes.Parent.ToString();
                }

                if (!string.IsNullOrEmpty(loginRequest.SchoolUrl))
                    schoolSettings = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl.ToLower() == loginRequest.SchoolUrl.ToLower());

                res.Result = new LoginSuccessResponse();
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id, permisions, schoolSettings, firstName, lastName, clientId, userType);
                res.Result.UserDetail = new UserDetail(schoolSettings, userAccount, firstName, lastName, id, userType);
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = "Unexpected Error Occurred";
                return res;
            }
        }

        async Task<APIResponse<LoginSuccessResponse>> IIdentityService.MobileLoginAsync(LoginCommand loginRequest)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            res.Result = new LoginSuccessResponse();
            string firstName = string.Empty;
            string lastName = string.Empty;
            string userType = string.Empty;
            try
            {
                var id = Guid.NewGuid();
                var clientId = ClientId(loginRequest.SchoolUrl);
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
               
                if (loginRequest.UserType == (int)UserTypes.Teacher)
                {
                    var teacher = GetTeacherByUserId(userAccount.Id, clientId);
                    if (teacher is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }

                    if (teacher.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }

                    if (utilitiesService.IsThisUser(UserTypes.Admin, userAccount.UserTypes))
                    {
                        permisions = context.AppActivity
                            .Where(d => d.IsActive).Select(s => s.Permission).Distinct().OrderBy(s => s).Distinct().ToList();
                        userType = UserTypes.Admin.ToString();
                    }
                    else
                    {
                        var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                        permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive & userRoleIds.Contains(d.RoleId) && d.ClientId == clientId).Select(s => s.Activity.Permission).Distinct().ToList();
                        userType = UserTypes.Teacher.ToString();
                    }
                    id = teacher.TeacherId;
                    firstName = teacher.FirstName;
                    lastName = teacher.LastName;
                }


                if (loginRequest.UserType == (int)UserTypes.Student)
                {
                    var student = GetStudentByUserId(userAccount.Id, clientId);
                    if (student is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }
                    if (student != null && student.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = student?.StudentContactId ?? new Guid();
                    firstName = student.FirstName;
                    lastName = student.LastName;
                    userType = UserTypes.Student.ToString();
                }
                SchoolSetting appSettings = null;

                if (loginRequest.UserType == (int)UserTypes.Parent)
                {
                    var parent = GetParentByUserId(userAccount.Id, clientId);
                    if (parent is null)
                    {
                        res.Message.FriendlyMessage = $"{loginRequest.UserName} is not available in school database";
                        return res;
                    }
                    firstName = parent.FirstName;
                    lastName = parent.LastName;
                    id = parent.Parentid;
                    userType = UserTypes.Parent.ToString();
                }

                if (!string.IsNullOrEmpty(loginRequest.SchoolUrl))
                    appSettings = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl.ToLower() == loginRequest.SchoolUrl.ToLower());

                var schoolSetting = context.SchoolSettings.FirstOrDefault(x => x.ClientId == appSettings.ClientId) ?? new SchoolSetting();

                res.Result = new LoginSuccessResponse();
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id, permisions, appSettings, firstName, lastName, clientId, userType);
                res.Result.UserDetail = new UserDetail(schoolSetting, userAccount, firstName, lastName, id, userType);
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<APIResponse<List<string>>> IIdentityService.GetTeacherMobilePermissionsAsync(string userId)
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

                if (utilitiesService.IsThisUser(UserTypes.Admin, userAccount.UserTypes))
                {
                    permisions = context.AppActivity.Where(d => d.IsActive).Select(s => s.Permission).OrderBy(s => s).Distinct().ToList();
                }
                else
                {
                    var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                    permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive
                    & d.Deleted == false
                    & userRoleIds.Contains(d.RoleId)).Select(s => s.Activity.Permission).Distinct().ToList();
                }
                var techerAccount = await context.Teacher.FirstOrDefaultAsync(e => e.UserId == userAccount.Id);
                if (techerAccount != null && techerAccount.Status == (int)TeacherStatus.Inactive)
                {
                    res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                    return res;
                }
                id = techerAccount.TeacherId;

                res.Result = permisions;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
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
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(AppUser user, Guid ID, List<string> permissions = null, SchoolSetting schoolSetting = null, string firstName = "", string lastName = "", string clientId = "", string userType = "")
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
                    new Claim("userType", userType),
                    new Claim("userName",user.UserName),
                    new Claim("name", firstName + " " + lastName),
                    userType == UserTypes.Teacher.ToString() ? new Claim("teacherId", ID.ToString()) : new Claim("teacherId", ID.ToString()),
                    userType == UserTypes.Student.ToString() ? new Claim("studentContactId", ID.ToString()) : new Claim("studentContactId", ID.ToString()),
                    userType == UserTypes.Admin.ToString() ? new Claim("teacherId", ID.ToString()) : new Claim("teacherId", ID.ToString()),
                    userType == UserTypes.Parent.ToString() ? new Claim("parentId", ID.ToString()) : new Claim("parentId", ID.ToString()),
                    permissions != null ? new Claim("permissions", string.Join(',', permissions)) : new Claim("permissions", string.Join(',', "N/A")),
                    schoolSetting != null ? new Claim("smsClientId", schoolSetting.ClientId) : new Claim("smsClientId", clientId),
                };
                accessor.HttpContext.Items["smsClientId"] = clientId;

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
                    SchoolUrl = schoolSetting.APPLAYOUTSETTINGS_SchoolUrl
                };
            }
            catch (Exception ex)
            {

                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw new ArgumentException($"Something went wrong: {ex.InnerException.Message}");
            }

        }

        public async Task<AppUser> FetchLoggedInUserDetailsAsync(string userId)
        {
            var currentUser = await userManager?.FindByIdAsync(userId);
            return currentUser; 
        }

        async Task<APIResponse<LoginSuccessResponse>> IIdentityService.LoginAfterPasswordIsChangedAsync(AppUser userAccount, string schoolUrl, UserTypes uType)
        {
            var res = new APIResponse<LoginSuccessResponse>();
            res.Result = new LoginSuccessResponse();
            try
            {
                var id = Guid.NewGuid();
                var permisions = new List<string>();
                SchoolSetting schoolSettings = null;
                string firstName = String.Empty;
                string lastName = String.Empty;
                string userType = string.Empty;

                var clientId = ClientId(schoolUrl);
                if (!string.IsNullOrEmpty(schoolUrl))
                    schoolSettings = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl.ToLower() == schoolUrl.ToLower());
                else
                {
                    res.Message.FriendlyMessage = $"Invalid request on login";
                    return res;
                }


                if (uType == UserTypes.Teacher)
                {
                    var teacher = GetTeacherByUserId(userAccount.Id, clientId);
                  
                    if (teacher.Status == (int)TeacherStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Teacher account is currently unavailable!! Please contact school administration";
                        return res;
                    }

                    if (utilitiesService.IsThisUser(UserTypes.Admin, userAccount.UserTypes))
                    {
                        permisions = context.AppActivity
                            .Where(d => d.IsActive).Select(s => s.Permission).Distinct().OrderBy(s => s).Distinct().ToList();
                        userType = UserTypes.Admin.ToString();
                    }
                    else
                    {
                        var userRoleIds = await context.UserRoles.Where(d => d.UserId == userAccount.Id).Select(d => d.RoleId).ToListAsync();
                        permisions = context.RoleActivity.Include(d => d.Activity).Where(d => d.Activity.IsActive & userRoleIds.Contains(d.RoleId) && d.ClientId == clientId).Select(s => s.Activity.Permission).Distinct().ToList();
                        userType = UserTypes.Teacher.ToString();
                    }
                    id = teacher.TeacherId;
                    firstName = teacher.FirstName;
                    lastName = teacher.LastName;
                }


                if (uType == UserTypes.Student)
                {

                    var student = GetStudentByUserId(userAccount.Id, clientId);
                    if (student != null && student.Status == (int)StudentStatus.Inactive)
                    {
                        res.Message.FriendlyMessage = $"Student account is currently unavailable!! Please contact school administration";
                        return res;
                    }
                    id = student?.StudentContactId ?? new Guid();
                    firstName = student.FirstName;
                    lastName = student.LastName;
                    userType = UserTypes.Student.ToString();
                }
                if (uType == UserTypes.Parent)
                {
                    var parent = GetParentByUserId(userAccount.Id, clientId);
                    firstName = parent.FirstName;
                    lastName = parent.LastName;
                    id = parent.Parentid;
                    userType = UserTypes.Parent.ToString();
                }



                if (!string.IsNullOrEmpty(schoolUrl))
                    schoolSettings = await context.SchoolSettings.FirstOrDefaultAsync(x => x.APPLAYOUTSETTINGS_SchoolUrl.ToLower() == schoolUrl.ToLower());

                var schoolSetting = context.SchoolSettings.FirstOrDefault(x => x.ClientId == schoolSettings.ClientId) ?? new SchoolSetting();

                res.Result = new LoginSuccessResponse();
                userAccount.EmailConfirmed = true;
                res.Result.AuthResult = await GenerateAuthenticationResultForUserAsync(userAccount, id, permisions, schoolSettings, firstName, lastName, clientId);
                res.Result.UserDetail = new UserDetail(schoolSetting, userAccount, firstName, lastName, id, userType);
                res.IsSuccessful = true;

                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
        }

        async Task<APIResponse<CBTLoginDetails>> IIdentityService.GetCBTTokenAsync()
        {

            var res = new APIResponse<CBTLoginDetails>();
            var apiCredentials = new SmsClientInformationRequest
            {
                ApiKey = fwsOptions.Apikey,
                ClientId = smsClientId
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

        string ClientId(string url) => context.SchoolSettings.FirstOrDefault(x => x.APPLAYOUTSETTINGS_SchoolUrl == url).ClientId;

        StudentDto GetStudentByUserId(string userId, string clientId) => context.StudentContact
            .Where(x => x.UserId == userId && x.ClientId == clientId && x.Deleted == false).Select(c => new StudentDto(c)).FirstOrDefault();

        ParentDto GetParentByUserId(string userId, string clientId) => context.Parents
            .Where(x => x.ClientId == clientId && x.UserId == userId && x.Deleted == false).Select(x => new ParentDto(x)).FirstOrDefault();

        TeacherDto GetTeacherByUserId(string userId, string clientId) => context.Teacher
            .Where(x => x.UserId == userId && x.ClientId == clientId && x.Deleted == false)
            .Select(c => new TeacherDto(c)).FirstOrDefault();

    }
}

