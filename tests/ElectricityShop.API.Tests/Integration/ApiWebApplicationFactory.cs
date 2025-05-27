using ElectricityShop.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace ElectricityShop.API.Tests.Integration
{
    // Use Program type instead of string "Program" to avoid inaccessible error
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real database context registrations
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var identityDbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>));

                if (identityDbContextDescriptor != null)
                {
                    services.Remove(identityDbContextDescriptor);
                }

                // Remove real messaging registrations
                var rabbitMQConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection));

                if (rabbitMQConnectionDescriptor != null)
                {
                    services.Remove(rabbitMQConnectionDescriptor);
                }

                var eventBusDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ));

                if (eventBusDescriptor != null)
                {
                    services.Remove(eventBusDescriptor);
                }

                // Directly create mocks without factory methods
                // Avoiding optional parameters in expression trees
                services.AddSingleton<ApplicationDbContext>(serviceProvider =>
                {
                    var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
                    var mock = new Mock<ApplicationDbContext>(options);
                    return mock.Object;
                });

                services.AddSingleton<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>(serviceProvider =>
                {
                    var options = new DbContextOptionsBuilder<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>().Options;
                    var mock = new Mock<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>(options);
                    return mock.Object;
                });

                // Mock messaging services - avoid expression trees with optional arguments
                services.AddSingleton<ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection>(serviceProvider =>
                {
                    return CreateRabbitMQConnectionMock();
                });

                services.AddSingleton<ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ>(serviceProvider =>
                {
                    return CreateEventBusMock();
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var logger = scopedServices.GetRequiredService<ILogger<ApiWebApplicationFactory>>();

                    try
                    {
                        // Seed the database with test data if needed
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        // Helper methods to create mocks outside of expression trees
        private static ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection CreateRabbitMQConnectionMock()
        {
            // Create a mock without using expression trees with optional parameters
            var mock = new Mock<ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection>();
            
            // Create a completed task outside the expression tree
            Task<bool> completedTask = Task.FromResult(true);
            
            // Use the created task in the setup with CancellationToken parameter
            mock.Setup(x => x.TryConnectAsync(It.IsAny<CancellationToken>())).Returns(completedTask);
                
            return mock.Object;
        }

        private static ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ CreateEventBusMock()
        {
            // Create a mock without using expression trees with optional parameters
            var mock = new Mock<ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ>();
            
            // Create a completed task outside the expression tree
            Task completedTask = Task.FromResult(true);
            
            // Use the created task in the setup with CancellationToken parameter
            mock.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>())).Returns(completedTask);
                
            return mock.Object;
        }
    }
}