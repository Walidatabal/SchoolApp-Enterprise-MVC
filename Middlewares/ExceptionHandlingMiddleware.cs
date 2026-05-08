using System.Net;
using System.Text.Json;

namespace SchoolApp.Middlewares
{
    // Global Exception Middleware
    //
    // Catches unhandled exceptions across the application
    // and returns clean error responses instead of crashing.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue request pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log full exception
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred.");

                // Return friendly JSON response
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                Success = false,
                Message = "An unexpected error occurred."
            };

            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}