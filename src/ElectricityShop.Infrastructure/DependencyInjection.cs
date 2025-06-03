using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Events;
using ElectricityShop.Domain.Events.Orders;
using ElectricityShop.Domain.Interfaces;
using ElectricityShop.Infrastructure.Events;
using ElectricityShop.Infrastructure.Persistence;
using ElectricityShop.Infrastructure.Persistence.Context;
using ElectricityShop.Infrastructure.Persistence.Repositories;
using ElectricityShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ElectricityShop.Infrastructure
{
    /// <summary>
    /// Extension methods for setting up infrastructure services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register infrastructure services
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register application services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFailedMessageLogger, FailedMessageLogger>();

            // Add event bus services
            services.AddRabbitMqEventBus(configuration);

            // Register event handlers
            services.AddEventHandlers(Assembly.GetExecutingAssembly(), typeof(DomainEvent).Assembly);

            // Register specific event types
            services.RegisterEventType<OrderPlacedEvent>();
            services.RegisterEventType<OrderStatusChangedEvent>();
            services.RegisterEventType<OrderCancelledEvent>();

            // Add the dead letter queue processor
            services.AddHostedService<DeadLetterQueueProcessor>();

            return services;
        }
    }
}