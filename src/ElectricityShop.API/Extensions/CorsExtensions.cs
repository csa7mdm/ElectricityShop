using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityShop.API.Extensions
{
    /// <summary>
    /// Extensions for configuring CORS policies
    /// </summary>
    public static class CorsExtensions
    {
        /// <summary>
        /// Adds and configures CORS policies for the application
        /// </summary>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            // Get allowed origins from configuration
            var corsSettings = configuration.GetSection("CorsSettings");
            var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };

            services.AddCors(options =>
            {
                // Default policy
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

                // Public API policy - more restrictive
                options.AddPolicy("PublicApi", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .WithMethods("GET", "POST")
                        .WithHeaders("Authorization", "Content-Type")
                        .AllowCredentials();
                });

                // Admin API policy - less restrictive
                options.AddPolicy("AdminApi", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            return services;
        }
    }
}