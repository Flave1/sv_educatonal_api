using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMP.DAL.Models.Logger;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Options
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                var errorResponse = new object();

                foreach (var error in errorsInModelState)
                {
                    foreach (var errorValue in error.Value)
                    {
                        var errorModel = new 
                        {
                            FieldName = error.Key,
                            FriendlyMessage = errorValue
                        };
                        errorResponse = errorModel;
                        break;
                    }
                    if (errorResponse == null)
                        continue;
                    break;
                }
                //LogError(errorResponse.ToString());
                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }
            await next();
        }

        private void LogError(string message)
        {
            var log = new Log
            {
                LogType = 1,
                Message = message,
                StackTrace = message,
                InnerException = message,
                InnerExceptionMessage = message
            };
            using(DataContext ctx = new DataContext())
            {
                ctx.Add(log);
                ctx.SaveChangesNoClientAsync();
            }
            
        }
    }
}
