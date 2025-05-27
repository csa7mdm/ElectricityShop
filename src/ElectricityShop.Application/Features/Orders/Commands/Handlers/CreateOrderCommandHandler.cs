using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Models; // For OrderStatus (Application layer)
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Enums; // For OrderStatus, PaymentMethod (Domain layer)
using ElectricityShop.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Orders.Commands.Handlers
{
    /// <summary>
    /// Handler for creating a new order with asynchronous processing
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderCreationResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IBackgroundJobService _backgroundJobs;
        private readonly IOrderProcessingService _orderProcessing;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the CreateOrderCommandHandler
        /// </summary>
        public CreateOrderCommandHandler(
            IApplicationDbContext dbContext,
            IBackgroundJobService backgroundJobs,
            IOrderProcessingService orderProcessing,
            IMediator mediator,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _backgroundJobs = backgroundJobs ?? throw new ArgumentNullException(nameof(backgroundJobs));
            _orderProcessing = orderProcessing ?? throw new ArgumentNullException(nameof(orderProcessing));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Handles the order creation command
        /// </summary>
        public async Task<OrderCreationResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new order for user {UserId}", request.UserId);
            
            try
            {
                // Perform validation
                // This occurs synchronously as part of the HTTP request

                // Construct shippingAddress first
                var shippingAddress = new Address
                {
                    Street = request.ShippingAddress.Line1,
                    StreetLine2 = request.ShippingAddress.Line2,
                    City = request.ShippingAddress.City,
                    State = request.ShippingAddress.State,
                    ZipCode = request.ShippingAddress.PostalCode,
                    Country = request.ShippingAddress.Country,
                    UserId = request.UserId
                };

                // Determine billingAddress next
                Address determinedBillingAddress;
                if (request.BillingAddress != null)
                {
                    determinedBillingAddress = new Address
                    {
                        Street = request.BillingAddress.Line1,
                        StreetLine2 = request.BillingAddress.Line2,
                        City = request.BillingAddress.City,
                        State = request.BillingAddress.State,
                        ZipCode = request.BillingAddress.PostalCode,
                        Country = request.BillingAddress.Country,
                        UserId = request.UserId
                    };
                }
                else
                {
                    // If BillingAddress is null, create a new Address instance from shipping for BillingAddress
                    determinedBillingAddress = new Address 
                    {
                        Street = shippingAddress.Street,
                        StreetLine2 = shippingAddress.StreetLine2,
                        City = shippingAddress.City,
                        State = shippingAddress.State,
                        ZipCode = shippingAddress.ZipCode,
                        Country = shippingAddress.Country,
                        UserId = shippingAddress.UserId
                    };
                }

                // Now create the order, including addresses in the initializer
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = GenerateOrderNumber(),
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow, // From BaseEntity or IAuditable
                    OrderDate = DateTime.UtcNow, // Explicitly set OrderDate
                    ShippingMethod = request.ShippingMethod,
                    Notes = request.Notes,
                    ShippingAddress = shippingAddress, // Assigned in initializer
                    BillingAddress = determinedBillingAddress, // Assigned in initializer
                    // PaymentMethod and Status will be set after this block as they involve parsing/mapping
                };

                // Set PaymentMethod (as before)
                if (Enum.TryParse<Domain.Enums.PaymentMethod>(request.PaymentMethod, true, out var paymentMethodEnum))
                {
                    order.PaymentMethod = paymentMethodEnum;
                }
                else
                {
                    _logger.LogError("Invalid payment method received: {PaymentMethod}", request.PaymentMethod);
                    throw new ArgumentException($"Invalid payment method: {request.PaymentMethod}");
                }

                // Set OrderStatus (as before)
                order.Status = (Domain.Enums.OrderStatus)(int)Models.OrderStatus.PaymentPending;
                
                // Add order items
                foreach (var item in request.Items)
                {
                    // Fetch product details from database to ensure accuracy of prices
                    var product = await _dbContext.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
                    
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product not found: {item.ProductId}");
                    }
                    
                    // Check if there's enough stock
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
                    }
                    
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                        // TotalPrice removed, OrderItem.Subtotal (get-only) will calculate this
                    };
                    
                    order.Items.Add(orderItem);
                }
                
                // Calculate order totals
                order.Subtotal = order.Items.Sum(i => i.Subtotal); // Changed from i.TotalPrice to i.Subtotal
                // Calculate tax, shipping, discounts as needed
                order.Tax = Math.Round(order.Subtotal * 0.1m, 2); // Example tax calculation
                order.ShippingCost = Math.Round(order.Subtotal * 0.05m, 2); // Example shipping calculation
                order.TotalAmount = order.Subtotal + order.Tax + order.ShippingCost;
                
                // Save order to database
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Order created with ID {OrderId} and number {OrderNumber}", 
                    order.Id, order.OrderNumber);
                
                // Publish domain event
                await _mediator.Publish(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerId = order.UserId
                }, cancellationToken);
                
                // Schedule background jobs for processing
                // First, process payment
                string paymentJobId = _backgroundJobs.Enqueue<IOrderProcessingService>(
                    service => service.ProcessPaymentAsync(order.Id));
                
                _logger.LogInformation("Payment processing job scheduled with ID {JobId}", paymentJobId);
                
                // After payment succeeds, send confirmation email
                _backgroundJobs.ContinueJobWith<IOrderProcessingService>(
                    paymentJobId,
                    service => service.SendOrderConfirmationEmailAsync(order.Id));
                
                // Also after payment, update inventory
                _backgroundJobs.ContinueJobWith<IOrderProcessingService>(
                    paymentJobId,
                    service => service.UpdateInventoryAsync(order.Id));
                
                // Finally, finalize the order
                _backgroundJobs.ContinueJobWith<IOrderProcessingService>(
                    paymentJobId,
                    service => service.FinalizeOrderAsync(order.Id));
                
                // Return order information immediately
                return new OrderCreationResult
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    // Map Domain.Enums.OrderStatus back to Application.Features.Orders.Models.OrderStatus
                    Status = (Models.OrderStatus)(int)order.Status,
                    TrackingId = paymentJobId,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(5) // Estimate for processing time
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", request.UserId);
                throw; // Let the global exception handler handle it
            }
        }
        
        /// <summary>
        /// Generates a unique order number
        /// </summary>
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}