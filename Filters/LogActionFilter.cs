using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolApp.Filters
{
    public class LogActionFilter : IActionFilter  // Implementing IActionFilter to create a custom action filter
    {
        private readonly IAppLogger _logger;

        public LogActionFilter(IAppLogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context) // This method is called before the action method is executed
        {
            _logger.LogInfo($"Action executing: {context.ActionDescriptor.DisplayName}");  // Log the name of the action being executed
        }

        public void OnActionExecuted(ActionExecutedContext context)  // This method is called after the action method has executed
        {
            _logger.LogInfo($"Action executed: {context.ActionDescriptor.DisplayName}");  // Log the name of the action that has been executed
        }
    }
}
