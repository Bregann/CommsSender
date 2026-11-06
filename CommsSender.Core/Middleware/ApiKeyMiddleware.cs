using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace CommsSender.Core.Middleware
{
    /// <summary>
    /// Two API keys, both sent in the X-CommsSender-ApiKey header:
    /// - CommsSenderApiKey (server key): full access. Used by your other services via CommsSenderClient.
    /// - CommsSenderAppApiKey (app key): may only register push tokens. It ships inside the mobile app,
    ///   so anyone can extract it, which is why it can't send messages.
    /// </summary>
    public class ApiKeyMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        private const string ApiKeyHeaderName = "X-CommsSender-ApiKey";
        private const string SwaggerPath = "/swagger";

        private static readonly PathString[] AppKeyAllowedPaths =
        [
            new PathString("/api/PushToken/RegisterPushToken")
        ];

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key validation for Swagger in development
            if (environment.IsDevelopment() && context.Request.Path.StartsWithSegments(SwaggerPath))
            {
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                Log.Warning("API key header not provided");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "API key is missing" });
                return;
            }

            string providedKey = extractedApiKey.ToString();

            if (KeyMatches(providedKey, Environment.GetEnvironmentVariable("CommsSenderApiKey")))
            {
                await next(context);
                return;
            }

            if (KeyMatches(providedKey, Environment.GetEnvironmentVariable("CommsSenderAppApiKey")))
            {
                if (IsAppKeyAllowed(context.Request.Path))
                {
                    await next(context);
                    return;
                }

                Log.Warning("App API key used for {Path}, which needs the server key", context.Request.Path.Value);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "This API key can only register push tokens" });
                return;
            }

            Log.Warning("Invalid API key provided");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid API key" });
        }

        private static bool IsAppKeyAllowed(PathString path)
        {
            return AppKeyAllowedPaths.Any(allowed => path.StartsWithSegments(allowed, StringComparison.OrdinalIgnoreCase));
        }

        private static bool KeyMatches(string providedKey, string? expectedKey)
        {
            if (string.IsNullOrEmpty(expectedKey))
            {
                return false;
            }

            // Constant-time comparison so response timing doesn't leak how much of a key was right
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(providedKey), Encoding.UTF8.GetBytes(expectedKey));
        }
    }
}
