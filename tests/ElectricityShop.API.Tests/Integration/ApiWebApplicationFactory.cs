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

                // Mock the database contexts
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: "InMemoryTestDb");
                });

                services.AddDbContext<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: "InMemoryIdentityDb");
                });

                // Mock messaging services
                services.AddSingleton<ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection>(serviceProvider =>
                {
                    var mock = new Mock<ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection>();
                    mock.Setup(x => x.TryConnectAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);
                    return mock.Object;
                });

                services.AddSingleton<ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ>(serviceProvider =>
                {
                    var mock = new Mock<ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ>();
                    mock.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
                    return mock.Object;
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to initialize the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<ApiWebApplicationFactory>>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    try
                    {
                        // Initialize the database with test data if needed
                        InitializeTestDatabase(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        private static void InitializeTestDatabase(ApplicationDbContext context)
        {
            // Add test data here as needed
        }
    }
}