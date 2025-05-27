using ElectricityShop.API.Middleware;

namespace ElectricityShop.API.Extensions
{
    /// <summary>
    /// Extensions for configuring security
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Adds security services
        /// </summary>
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            // Add anti-forgery services
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "__Host-X-XSRF-TOKEN";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Add HTTP strict transport security
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            return services;
        }

        /// <summary>
        /// Adds security headers middleware to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.UseMiddleware<SecurityHeadersMiddleware>();
            return app;
        }

        /// <summary>
        /// Adds request forgery protection to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseRequestForgeryProtection(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // For API endpoints, check for the presence of the X-XSRF-TOKEN header
                // This is for AJAX requests
                if (context.Request.Method != HttpMethods.Get &&
                    context.Request.Method != HttpMethods.Head &&
                    context.Request.Method != HttpMethods.Options &&
                    !context.Request.Headers.ContainsKey("X-XSRF-TOKEN"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Invalid anti-forgery token.");
                    return;
                }

                await next();
            });

            return app;
        }
    }
}