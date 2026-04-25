using Microsoft.Extensions.Logging;

namespace NewProjectFromScratch.Infrastructure.Logging
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            await _next(context);
            var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms", context.Request.Method, context.Request.Path + context.Request.QueryString, context.Response.StatusCode, elapsedMs);
        }
    }
}
