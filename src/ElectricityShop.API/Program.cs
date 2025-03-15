using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ElectricityShop.API.Extensions;
using ElectricityShop.Application.Common.Behaviors;
using ElectricityShop.Infrastructure.Extensions;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI with custom schema ID provider
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    // Use fully qualified type names to avoid schema ID conflicts
    c.CustomSchemaIds(type => {
        // Handle nested classes by incorporating parent class name
        if (type.DeclaringType != null)
        {
            return $"{type.DeclaringType.Name}_{type.Name}";
        }
        return type.Name;
    });
});

// Add application and infrastructure services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ElectricityShop.Application.Features.Products.Commands.CreateProductCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(ElectricityShop.Application.Features.Products.Commands.CreateProductCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddAutoMapper(typeof(ElectricityShop.Application.Features.Products.Commands.CreateProductCommand).Assembly);

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add authorization policies
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

// Automatically apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var appContext = services.GetRequiredService<ElectricityShop.Infrastructure.Persistence.Context.ApplicationDbContext>();
        appContext.Database.Migrate();
        
        var identityContext = services.GetRequiredService<ElectricityShop.Infrastructure.Identity.Context.AppIdentityDbContext>();
        identityContext.Database.Migrate();
        
        // Initialize RabbitMQ connection
        var rabbitMQConnection = services.GetRequiredService<ElectricityShop.Infrastructure.Messaging.RabbitMQ.RabbitMQConnection>();
        rabbitMQConnection.TryConnectAsync().GetAwaiter().GetResult();
        
        // Initialize EventBus
        var eventBus = services.GetRequiredService<ElectricityShop.Infrastructure.Messaging.RabbitMQ.EventBusRabbitMQ>();
        eventBus.InitializeAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
