using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Infrastructure.BackgroundJobs;
using ElectricityShop.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ElectricityShop.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions for registering asynchronous processing services
    /// </summary>
    public static class AsyncProcessingExtensions
    {
        /// <summary>
        /// Adds asynchronous processing capabilities to the service collection
        /// </summary>
        public static IServiceCollection AddAsyncProcessing(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            
            // Set worker count from configuration
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = configuration.GetValue<int>("Hangfire:WorkerCount", 5);
                options.ServerName = $"ElectricityShop-{Environment.MachineName}";
            });
            
            // Register global Hangfire filters
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 }); // Disable automatic retries, we'll handle them manually
            
            // Register service provider for logging in the global filter
            var serviceProvider = services.BuildServiceProvider();
            GlobalJobFilters.Filters.Add(new HangfireErrorHandler(
                serviceProvider.GetRequiredService<ILogger<HangfireErrorHandler>>()));
            
            // Register background job service
            services.AddSingleton<IBackgroundJobService, HangfireJobService>();
            
            // Register retry policy service
            services.AddSingleton<RetryPolicyService>();
            
            // Register order processing service
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            
            // Register payment service
            services.Configure<PaymentSettings>(configuration.GetSection("Payment"));
            services.AddScoped<IPaymentService, StripePaymentService>();
            
            // Register email service
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:SenderName"])
                .AddRazorRenderer()
                .AddSmtpSender(configuration["Email:Host"], 
                              configuration.GetValue<int>("Email:Port"), 
                              configuration["Email:Username"],
                              configuration["Email:Password"]);
            services.AddScoped<IEmailService, SmtpEmailService>();
            
            return services;
        }
    }
}