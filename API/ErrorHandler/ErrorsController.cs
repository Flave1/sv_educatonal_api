using BLL;
using BLL.LoggerService;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SMP.BLL.Constants;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.CustomError
{
    [Route("/errors")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : Controller
    {
        private readonly ILoggerService _logger;
        public ErrorsController(ILoggerService loggerService)
        {
            _logger = loggerService;
        }
        [Route("{code}")]
        public IActionResult GenerateErrorMessage(int? code)
        {
            BaseApiError error;
            HttpContext context = HttpContext;
            HttpStatusCode parseCode = (HttpStatusCode)code;
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                var exception = exceptionHandlerPathFeature.Error;

                var result = JsonConvert.SerializeObject(exception.Message);

                context.Response.ContentType = "application/json";

                if (parseCode == HttpStatusCode.NotFound)
                {
                    error = new BaseApiError(code ?? 0, parseCode.ToString(), Messages.FriendlyNOTFOUND);
                    _logger.Error($"ErrorsController {exception?.Message}", exception?.StackTrace, exception?.InnerException.ToString(), exception?.InnerException?.Message);

                    return new ObjectResult(new APIResponse<APIResponseMessage>
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = error.Friendly_Message,
                            TechnicalMessage = error.Technical_Message
                        }
                    });
                }
                if (parseCode == HttpStatusCode.InternalServerError)
                {
                    error = new BaseApiError(code ?? 0, parseCode.ToString(), Messages.FriendlyException, result);
                    _logger.Error($"ErrorsController {exception?.Message}", exception?.StackTrace, exception?.InnerException?.ToString(), exception?.InnerException?.Message);
                    return new ObjectResult(new APIResponse<APIResponseMessage>
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = error.Friendly_Message,
                            TechnicalMessage = error.Technical_Message
                        }
                    });
                }

                if (parseCode == HttpStatusCode.Forbidden)
                {
                    error = new BaseApiError(code ?? 0, parseCode.ToString(), Messages.FriendlyForbidden, result);
                    _logger.Error($"ErrorsController {exception?.Message}", exception?.StackTrace, exception?.InnerException.ToString(), exception?.InnerException?.Message);
                    return new ObjectResult(new APIResponse<APIResponseMessage>
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = error.Friendly_Message,
                            TechnicalMessage = error.Technical_Message
                        }
                    });
                }

                error = new BaseApiError(code ?? 0, parseCode.ToString(), result);
                _logger.Error($"ErrorsController {exception?.Message}", exception?.StackTrace, exception?.InnerException.ToString(), exception?.InnerException?.Message);
                return new ObjectResult(new APIResponse<APIResponseMessage>
                {
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = error.Friendly_Message,
                        TechnicalMessage = error.Technical_Message
                    }
                });
            }
            if (parseCode == HttpStatusCode.NotFound)
            {
                error = new BaseApiError(code ?? 0, parseCode.ToString(), Messages.FriendlyNOTFOUND);
                _logger.Information($"ErrorsController {error.Technical_Message}");
                return new ObjectResult(new APIResponse<APIResponseMessage>
                {
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = error.Friendly_Message,
                        TechnicalMessage = error.Technical_Message
                    }
                });
            }
            error = new BaseApiError(code ?? 0, parseCode.ToString());
            return new ObjectResult(new APIResponse<APIResponseMessage>
            {
                Message = new APIResponseMessage
                {
                    FriendlyMessage = error.Friendly_Message,
                    TechnicalMessage = error.Technical_Message
                }
            });
        }




    }
}
