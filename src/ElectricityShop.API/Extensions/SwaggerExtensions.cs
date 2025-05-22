using ElectricityShop.API.Extensions.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace ElectricityShop.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Electricity Shop API",
                    Version = "v1",
                    Description = "REST API for Electricity Shop e-commerce application",
                    Contact = new OpenApiContact
                    {
                        Name = "Electricity Shop Team",
                        Email = "support@electricityshop.com",
                        Url = new Uri("https://electricityshop.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under proprietary license",
                        Url = new Uri("https://electricityshop.com/license")
                    }
                });

                // Add JWT Authentication support in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Add rate limiting documentation
                c.AddSecurityDefinition("RateLimit", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-Rate-Limit-Limit",
                    In = ParameterLocation.Header,
                    Description = "API rate limit: 10 requests per 10 seconds for public endpoints, 5 requests per 5 minutes for authentication endpoints."
                });

                // Add examples
                c.SchemaFilter<ExampleSchemaFilter>();
                c.OperationFilter<AuthorizeOperationFilter>();

                // Use custom schema IDs
                c.CustomSchemaIds(type => {
                    if (type.DeclaringType != null)
                    {
                        return $"{type.DeclaringType.Name}_{type.Name}";
                    }
                    return type.Name;
                });

                // Enable XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electricity Shop API v1");
                c.RoutePrefix = string.Empty; // Serves the Swagger UI at the app's root
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.EnableFilter();
                c.DisplayRequestDuration();
            });

            return app;
        }
    }
}