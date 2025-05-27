using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityShop.API.Extensions
{
    /// <summary>
    /// Extensions for Native AOT compatibility
    /// </summary>
    public static class NativeAotExtensions
    {
        /// <summary>
        /// Configures services to be Native AOT compatible
        /// </summary>
        public static IServiceCollection AddNativeAotSupport(this IServiceCollection services)
        {
            // Configure JSON serializer for trimming
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });
            
            // Configure controllers JSON options for trimming
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });
            
            return services;
        }
    }
    
    /// <summary>
    /// JSON serializer context for AOT compilation
    /// </summary>
    [JsonSerializable(typeof(API.Models.PagedResponse<Application.Features.Products.Queries.ProductDto>))]
    [JsonSerializable(typeof(Application.Features.Products.Queries.ProductDto))]
    [JsonSerializable(typeof(Application.Features.Products.Commands.CreateProductCommand))]
    [JsonSerializable(typeof(Application.Features.Products.Commands.UpdateProductCommand))]
    [JsonSerializable(typeof(Application.Features.Products.Commands.DeleteProductCommand))]
    [JsonSerializable(typeof(Application.Features.Orders.Queries.OrderDto))]
    [JsonSerializable(typeof(List<Application.Features.Products.Queries.ProductDto>))]
    [JsonSerializable(typeof(List<Application.Features.Orders.Queries.OrderDto>))]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}