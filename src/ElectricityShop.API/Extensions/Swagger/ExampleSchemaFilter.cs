using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel;

namespace ElectricityShop.API.Extensions.Swagger
{
    /// <summary>
    /// Schema filter for adding examples to Swagger documentation
    /// </summary>
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema == null || context == null || context.Type == null)
                return;

            // Add examples based on type
            if (context.Type == typeof(ElectricityShop.API.Controllers.RegisterRequest))
            {
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("user@example.com"),
                    ["password"] = new OpenApiString("StrongP@ssw0rd!"),
                    ["firstName"] = new OpenApiString("John"),
                    ["lastName"] = new OpenApiString("Doe")
                };
            }
            else if (context.Type == typeof(ElectricityShop.API.Controllers.LoginRequest))
            {
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("user@example.com"),
                    ["password"] = new OpenApiString("StrongP@ssw0rd!")
                };
            }
            else if (context.Type == typeof(ElectricityShop.Application.Features.Products.Commands.CreateProductCommand))
            {
                schema.Example = new OpenApiObject
                {
                    ["name"] = new OpenApiString("Premium LED Light Bulb"),
                    ["description"] = new OpenApiString("Energy-efficient LED light bulb with 5000K color temperature"),
                    ["price"] = new OpenApiDouble(9.99),
                    ["stockQuantity"] = new OpenApiInteger(100),
                    ["categoryId"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                };
            }
            else if (context.Type == typeof(ElectricityShop.Application.Features.Products.Commands.UpdateProductCommand))
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    ["name"] = new OpenApiString("Premium LED Light Bulb V2"),
                    ["description"] = new OpenApiString("Updated energy-efficient LED light bulb with 5000K color temperature"),
                    ["price"] = new OpenApiDouble(12.99),
                    ["stockQuantity"] = new OpenApiInteger(150),
                    ["categoryId"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    ["isActive"] = new OpenApiBoolean(true)
                };
            }
            // Add more examples as needed
        }
    }
}