using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ElectricityShop.API.Extensions.Swagger
{
    /// <summary>
    /// Operation filter for handling authorization in Swagger documentation
    /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if operation has authorization attributes
            var authorizeAttributes = context.MethodInfo.GetCustomAttributes(true)
                .Union(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .ToList();

            if (!authorizeAttributes.Any())
                return;

            // Add authorization requirement
            operation.Security.Add(new OpenApiSecurityRequirement
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
                    new List<string>()
                }
            });

            // Add notes about required roles
            var roles = authorizeAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .SelectMany(a => a.Roles.Split(','))
                .Distinct()
                .ToList();

            if (roles.Any())
            {
                if (operation.Description == null)
                    operation.Description = string.Empty;

                operation.Description += 
                    $"\n\n**Authorization:** Requires {(roles.Count == 1 ? "role" : "one of these roles")}: " + 
                    string.Join(", ", roles.Select(r => $"`{r.Trim()}`"));
            }
        }
    }
}