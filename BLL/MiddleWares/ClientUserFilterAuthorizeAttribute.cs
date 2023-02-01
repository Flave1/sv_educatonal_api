﻿using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.MiddleWares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ClientUserFilterAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                StringValues authHeader = context.HttpContext.Request.Headers["Authorization"];

                bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
                if (context == null || hasAllowAnonymous)
                {
                    await next();
                    return;
                }
                if (string.IsNullOrEmpty(authHeader))
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

                var smsClientId = tokena.Claims.FirstOrDefault(x => x.Type == "smsClientId").Value ?? "";

                if (string.IsNullOrEmpty(smsClientId))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                    return;
                }
                context.HttpContext.Items["smsClientId"] = smsClientId;

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