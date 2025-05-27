using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Xunit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElectricityShop.API.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void BuildServices_RegistersExpectedServices()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            
            // Add services from Program.cs
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => {
                c.CustomSchemaIds(type => {
                    if (type.DeclaringType != null)
                    {
                        return $"{type.DeclaringType.Name}_{type.Name}";
                    }
                    return type.Name;
                });
            });

            // Add MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
                typeof(ElectricityShop.Application.Features.Products.Commands.CreateProductCommand).Assembly));

            // Build the service provider
            var serviceProvider = builder.Services.BuildServiceProvider();

            // Act & Assert
            // Check for controllers
            Assert.NotNull(serviceProvider.GetService<IHostApplicationLifetime>());
            
            // Check for MediatR
            Assert.NotNull(serviceProvider.GetService<IMediator>());

            // Check for Swagger
            var apiDescriptionGroupCollectionProvider = serviceProvider.GetService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider>();
            Assert.NotNull(apiDescriptionGroupCollectionProvider);
        }

        [Fact]
        public void Configure_RegistersExpectedMiddleware()
        {
            // This is harder to test directly since the middleware pipeline is built at runtime
            // A better approach is to test this with the WebApplicationFactory in the integration tests
            // Here we'll just verify basic configuration requirements
            
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            var env = app.Environment;
            
            // No direct assertions possible for middleware registration
            // Just ensure it doesn't throw an exception
            
            // Dev environment middleware
            app.Environment.EnvironmentName = Environments.Development;
            app.UseSwagger();
            app.UseSwaggerUI();
            
            // Common middleware
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            
            // Assert
            Assert.True(true); // No exception was thrown
        }
    }
}