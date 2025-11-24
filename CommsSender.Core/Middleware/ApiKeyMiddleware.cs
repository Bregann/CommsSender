using Serilog;

namespace CommsSender.Core.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            const string apiKeyHeaderName = "X-CommsSender-ApiKey";

            if (!context.Request.Headers.TryGetValue(apiKeyHeaderName, out var extractedApiKey))
            {
                Log.Warning("API key header not provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "API key is missing" });
                return;
            }

            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration["CommsSenderApiKey"];

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
