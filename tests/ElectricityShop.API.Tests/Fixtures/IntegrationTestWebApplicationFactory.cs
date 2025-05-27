using System;
using ElectricityShop.API;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Tests.Fixtures
{
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the database context with an in-memory database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                services.AddScoped<IApplicationDbContext>(provider => 
                    provider.GetRequiredService<ApplicationDbContext>());

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to initialize the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<IntegrationTestWebApplicationFactory>>();

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

        public void ConfigureTestServices(Action<IServiceCollection> configureServices)
        {
            ConfigureWebHost(webHostBuilder => webHostBuilder.ConfigureServices(configureServices));
        }

        private void InitializeTestDatabase(ApplicationDbContext context)
        {
            // Add test data to the database
            // For example, add test products, customers, etc.
        }
    }
}