using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CRUDOperations.Filters
{
    public class LogActivityFilter : IActionFilter, IAsyncActionFilter
    {
        private readonly ILogger<LogActivityFilter> _logger;

        public LogActivityFilter(ILogger<LogActivityFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"Executing Action `{context.ActionDescriptor.DisplayName}` on controller `{context.Controller}` with agrguments {JsonSerializer.Serialize(context.ActionArguments)}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Action `{context.ActionDescriptor.DisplayName}` finished execution on controller `{context.Controller}` ");

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation($"(Async) Executing Action `{context.ActionDescriptor.DisplayName}` on controller `{context.Controller}` with agrguments {JsonSerializer.Serialize(context.ActionArguments)}");
            await next();
            _logger.LogInformation($"(Async) Action `{context.ActionDescriptor.DisplayName}` finished execution on controller `{context.Controller}` ");
        }
    }
}
