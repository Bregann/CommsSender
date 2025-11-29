using Serilog;

namespace CommsSender.Core.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            const string apiKeyHeaderName = "X-CommsSender-ApiKey";
            const string swaggerPath = "/swagger";

            // Skip API key validation for Swagger in development
            if (environment.IsDevelopment() && context.Request.Path.StartsWithSegments(swaggerPath))
            {
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(apiKeyHeaderName, out var extractedApiKey))
            {
                Log.Warning("API key header not provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "API key is missing" });
                return;
            }

            var apiKey = Environment.GetEnvironmentVariable("CommsSenderApiKey");

            if (apiKey is null || !apiKey.Equals(extractedApiKey))
            {
                Log.Warning("Invalid API key provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API key" });
                return;
            }

            await next(context);
        }
    }
}
