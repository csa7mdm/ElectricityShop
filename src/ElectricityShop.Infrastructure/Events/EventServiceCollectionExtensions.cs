using System;
using System.Linq;
using System.Reflection;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// Extension methods for configuring event-related services
    /// </summary>
    public static class EventServiceCollectionExtensions
    {
        /// <summary>
        /// Adds RabbitMQ event bus services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddRabbitMqEventBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add RabbitMQ settings
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            
            // Add event type registry
            services.AddSingleton<EventTypeRegistry>();
            
            // Add RabbitMQ services
            services.AddSingleton<RabbitMqConnectionFactory>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddHostedService<RabbitMqEventConsumer>();
            
            return services;
        }

        /// <summary>
        /// Adds event handlers from specified assemblies
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="assemblies">The assemblies to scan for event handlers</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddEventHandlers(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            // Get all event handler implementations
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .ToList();

            // Register each handler
            foreach (var handlerType in handlerTypes)
            {
                // Get the event type from the handler interface
                var eventType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .GetGenericArguments()[0];

                // Register the handler with the container
                services.AddScoped(
                    typeof(IEventHandler<>).MakeGenericType(eventType),
                    handlerType);

                // Register the event type with the registry
                services.BuildServiceProvider()
                    .GetRequiredService<EventTypeRegistry>()
                    .RegisterEventType(eventType);
            }

            return services;
        }

        /// <summary>
        /// Adds a specific event handler
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="THandler">The event handler type</typeparam>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddEventHandler<TEvent, THandler>(
            this IServiceCollection services)
            where TEvent : IDomainEvent
            where THandler : class, IEventHandler<TEvent>
        {
            // Register the handler
            services.AddScoped<IEventHandler<TEvent>, THandler>();
            
            // Register the event type
            services.BuildServiceProvider()
                .GetRequiredService<EventTypeRegistry>()
                .RegisterEventType<TEvent>();
            
            return services;
        }

        /// <summary>
        /// Registers a domain event manually
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection RegisterEventType<TEvent>(
            this IServiceCollection services)
            where TEvent : IDomainEvent
        {
            services.BuildServiceProvider()
                .GetRequiredService<EventTypeRegistry>()
                .RegisterEventType<TEvent>();
            
            return services;
        }
    }
}