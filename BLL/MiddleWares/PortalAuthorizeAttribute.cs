using Contracts.Options;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.MiddleWares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PortalAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        { 
            string userId = context.HttpContext.User?.FindFirst("userId")?.Value ?? string.Empty;
            StringValues authHeader = context.HttpContext.Request.Headers["Authorization"];

            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (context == null || hasAllowAnonymous)
            {
                await next();
                return;
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(authHeader))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                return;
            }
            string token = authHeader.ToString().Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var tokena = handler.ReadJwtToken(token);
            var FromDate = tokena.IssuedAt.AddHours(1);
            var EndDate = tokena.ValidTo.AddHours(1);

            var currentDateTime = DateTime.UtcNow.AddHours(1);
            if (currentDateTime > EndDate)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                return;
            }

            using (var scope = context.HttpContext.RequestServices.CreateScope())
            {
                try
                {
                    IServiceProvider scopedServices = scope.ServiceProvider;
                    JwtSettings tokenSettings = scopedServices.GetRequiredService<JwtSettings>();
                    DataContext _dataContext = scopedServices.GetRequiredService<DataContext>();

                    context.HttpContext.Items["smsClientId"] = context.HttpContext.User?.FindFirst("smsClientId")?.Value ?? string.Empty;

                    await next();
                    return;
                }
                catch (Exception ex)
                {
                    context.HttpContext.Response.StatusCode = 500; 
                    return;
                }
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HubAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string name = context.HttpContext.User?.FindFirst("name")?.Value ?? string.Empty;
            StringValues authHeader = context.HttpContext.Request.Headers["Authorization"];

            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (context == null || hasAllowAnonymous)
            {
                await next();
                return;
            }
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(authHeader))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                return;
            }
            string token = authHeader.ToString().Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var tokena = handler.ReadJwtToken(token);
            var FromDate = tokena.IssuedAt.AddHours(1);
            var EndDate = tokena.ValidTo.AddHours(1);

            var currentDateTime = DateTime.UtcNow.AddHours(1);
            if (currentDateTime > EndDate)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                return;
            }

            using (var scope = context.HttpContext.RequestServices.CreateScope())
            {
                try
                {
                    IServiceProvider scopedServices = scope.ServiceProvider;
                    JwtSettings tokenSettings = scopedServices.GetRequiredService<JwtSettings>();
                    DataContext _dataContext = scopedServices.GetRequiredService<DataContext>();

                    var request = context.HttpContext.Items["UserConnection"];
                    context.HttpContext.Items["UserConnection"] = request;

                    await next();
                    return;
                }
                catch (Exception ex)
                {
                    context.HttpContext.Response.StatusCode = 500;
                    return;
                }
            }
        }

        public string FormatRequestBody(IDictionary<string, object> actionArguments)
        {
            try
            {

                if (actionArguments != null)
                    return $"{JsonConvert.SerializeObject(actionArguments)}";
            }
            catch (Exception ex)
            {

            }
            return "";
        }
    }
}
