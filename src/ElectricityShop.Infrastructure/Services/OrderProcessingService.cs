using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Models;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// Service for handling long-running order processing tasks
    /// </summary>
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IMediator _mediator;
        private readonly RetryPolicyService _retryPolicyService;
        private readonly ILogger<OrderProcessingService> _logger;
        
        /// <summary>
        /// Initializes a new instance of the OrderProcessingService
        /// </summary>
        public OrderProcessingService(
            IApplicationDbContext dbContext,
            IPaymentService paymentService,
            IEmailService emailService,
            IMediator mediator,
            RetryPolicyService retryPolicyService,
            ILogger<OrderProcessingService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _retryPolicyService = retryPolicyService ?? throw new ArgumentNullException(nameof(retryPolicyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Processes payment for an order
        /// </summary>
        public async Task ProcessPaymentAsync(Guid orderId)
        {
            _logger.LogInformation("Starting payment processing for order {OrderId}", orderId);
            
            try
            {
                // Get retry policy for payment operations
                var retryPolicy = _retryPolicyService.GetPaymentRetryPolicy();
                
                // Execute with retry policy
                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Get order details
                    var order = await _dbContext.Orders
                        .Include(o => o.Items)
                        .FirstOrDefaultAsync(o => o.Id == orderId);
                    
                    if (order == null)
                    {
                        _logger.LogError("Order {OrderId} not found when processing payment", orderId);
                        throw new InvalidOperationException($"Order {orderId} not found");
                    }
                    
                    // Verify order is in correct state
                    if (!order.Status.Equals(OrderStatus.PaymentPending))
                    {
                        _logger.LogWarning("Cannot process payment for order {OrderId} in status {Status}", 
                            orderId, order.Status);
                        return; // Skip payment processing if not in correct state
                    }
                    
                    // Convert string payment method to Guid for demo
                    var paymentMethodId = Guid.Parse(order.PaymentMethod);
                    
                    // Process payment
                    var paymentResult = await _paymentService.ProcessPaymentAsync(
                        orderId, 
                        order.TotalAmount, 
                        paymentMethodId);
                    
                    // Update order status based on payment result
                    if (paymentResult.Success)
                    {
                        order.Status = OrderStatus.PaymentProcessed;
                        order.PaymentTransactionId = paymentResult.TransactionId;
                        _logger.LogInformation("Payment processed successfully for order {OrderId}, Transaction ID: {TransactionId}", 
                            orderId, paymentResult.TransactionId);
                    }
                    else
                    {
                        order.Status = OrderStatus.Failed;
                        order.StatusNotes = $"Payment failed: {paymentResult.ErrorMessage}";
                        _logger.LogWarning("Payment failed for order {OrderId}: {ErrorMessage}", 
                            orderId, paymentResult.ErrorMessage);
                    }
                    
                    await _dbContext.SaveChangesAsync();
                    
                    // Publish domain event
                    await _mediator.Publish(new OrderPaymentProcessedEvent
                    {
                        OrderId = order.Id,
                        OrderNumber = order.OrderNumber,
                        PaymentSuccess = paymentResult.Success,
                        TransactionId = paymentResult.TransactionId,
                        ErrorMessage = paymentResult.ErrorMessage
                    });
                    
                    // If payment failed, throw exception to prevent continuations
                    if (!paymentResult.Success)
                    {
                        throw new PaymentFailedException(paymentResult.ErrorMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                // Catch exceptions not handled by retry policy
                // Update order status to failed if not already done
                var order = await _dbContext.Orders.FindAsync(orderId);
                if (order != null && order.Status != OrderStatus.Failed)
                {
                    order.Status = OrderStatus.Failed;
                    order.StatusNotes = $"Payment processing error: {ex.Message}";
                    await _dbContext.SaveChangesAsync();
                    
                    await _mediator.Publish(new OrderFailedEvent
                    {
                        OrderId = orderId,
                        OrderNumber = order.OrderNumber,
                        Reason = ex.Message
                    });
                }
                
                _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                throw; // Rethrow to mark job as failed in Hangfire
            }
        }
        
        /// <summary>
        /// Sends order confirmation email
        /// </summary>
        public async Task SendOrderConfirmationEmailAsync(Guid orderId)
        {
            _logger.LogInformation("Sending confirmation email for order {OrderId}", orderId);
            
            try
            {
                var retryPolicy = _retryPolicyService.GetEmailRetryPolicy();
                
                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Get order with customer details
                    var order = await _dbContext.Orders
                        .Include(o => o.User)
                        .Include(o => o.Items)
                        .FirstOrDefaultAsync(o => o.Id == orderId);
                    
                    if (order == null)
                    {
                        _logger.LogError("Order {OrderId} not found when sending confirmation email", orderId);
                        throw new InvalidOperationException($"Order {orderId} not found");
                    }
                    
                    // Skip if order failed
                    if (order.Status == OrderStatus.Failed)
                    {
                        _logger.LogWarning("Skipping confirmation email for failed order {OrderId}", orderId);
                        return;
                    }
                    
                    // Get user email - in a real app you'd have this in the User entity
                    var userEmail = order.User?.Email ?? "customer@example.com";
                    var userName = order.User?.UserName ?? "Customer";
                    
                    // Prepare order items for email
                    var orderItems = order.Items.Select(i => new OrderItemEmailModel
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList();
                    
                    // Send email
                    var emailResult = await _emailService.SendOrderConfirmationAsync(
                        userEmail,
                        userName,
                        order.OrderNumber,
                        orderItems,
                        order.TotalAmount);
                    
                    if (!emailResult.Sent)
                    {
                        _logger.LogWarning("Failed to send confirmation email for order {OrderId}: {ErrorMessage}", 
                            orderId, emailResult.ErrorMessage);
                        throw new EmailSendFailedException(emailResult.ErrorMessage);
                    }
                    
                    _logger.LogInformation("Confirmation email sent for order {OrderId}", orderId);
                });
            }
            catch (Exception ex)
            {
                // Log but don't rethrow - email failures shouldn't block order processing
                _logger.LogError(ex, "Error sending confirmation email for order {OrderId}", orderId);
            }
        }
        
        /// <summary>
        /// Updates inventory after successful payment
        /// </summary>
        public async Task UpdateInventoryAsync(Guid orderId)
        {
            _logger.LogInformation("Updating inventory for order {OrderId}", orderId);
            
            try
            {
                var retryPolicy = _retryPolicyService.GetDatabaseRetryPolicy();
                
                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Get order items
                    var order = await _dbContext.Orders
                        .Include(o => o.Items)
                        .FirstOrDefaultAsync(o => o.Id == orderId);
                    
                    if (order == null)
                    {
                        _logger.LogError("Order {OrderId} not found when updating inventory", orderId);
                        throw new InvalidOperationException($"Order {orderId} not found");
                    }
                    
                    // Skip if order failed or not paid
                    if (!order.Status.Equals(OrderStatus.PaymentProcessed))
                    {
                        _logger.LogWarning("Skipping inventory update for order {OrderId} in status {Status}", 
                            orderId, order.Status);
                        return;
                    }
                    
                    if (!order.Items.Any())
                    {
                        _logger.LogError("No items found for order {OrderId} when updating inventory", orderId);
                        throw new InvalidOperationException($"No items found for order {orderId}");
                    }
                    
                    // Update inventory for each product
                    foreach (var item in order.Items)
                    {
                        var product = await _dbContext.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity -= item.Quantity;
                            if (product.StockQuantity < 0)
                            {
                                product.StockQuantity = 0;
                                _logger.LogWarning("Product {ProductId} stock went negative for order {OrderId}", 
                                    product.Id, orderId);
                            }
                            
                            _logger.LogInformation("Updated inventory for product {ProductId}, new stock: {StockQuantity}", 
                                product.Id, product.StockQuantity);
                        }
                        else
                        {
                            _logger.LogWarning("Product {ProductId} not found when updating inventory for order {OrderId}", 
                                item.ProductId, orderId);
                        }
                    }
                    
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Inventory updated for order {OrderId}", orderId);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory for order {OrderId}", orderId);
                
                // Don't rethrow as this shouldn't fail the entire order process
                // Instead, flag for manual inventory adjustment
                try
                {
                    var order = await _dbContext.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.StatusNotes = (order.StatusNotes ?? "") + 
                            " | Inventory update failed: " + ex.Message;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Failed to update order notes for inventory failure on order {OrderId}", orderId);
                }
            }
        }
        
        /// <summary>
        /// Finalizes the order after all processing steps
        /// </summary>
        public async Task FinalizeOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Finalizing order {OrderId}", orderId);
            
            try
            {
                var retryPolicy = _retryPolicyService.GetDatabaseRetryPolicy();
                
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var order = await _dbContext.Orders.FindAsync(orderId);
                    if (order == null)
                    {
                        _logger.LogError("Order {OrderId} not found when finalizing", orderId);
                        throw new InvalidOperationException($"Order {orderId} not found");
                    }
                    
                    // Only finalize orders that have been payment processed
                    if (order.Status.Equals(OrderStatus.PaymentProcessed))
                    {
                        order.Status = OrderStatus.Fulfilled;
                        order.FulfilledAt = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync();
                        
                        await _mediator.Publish(new OrderFulfilledEvent
                        {
                            OrderId = order.Id,
                            OrderNumber = order.OrderNumber
                        });
                        
                        _logger.LogInformation("Order {OrderId} finalized successfully", orderId);
                    }
                    else
                    {
                        _logger.LogWarning("Cannot finalize order {OrderId} in status {Status}", 
                            orderId, order.Status);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing order {OrderId}", orderId);
                throw;
            }
        }
    }
    
    /// <summary>
    /// Exception thrown when payment processing fails
    /// </summary>
    public class PaymentFailedException : Exception
    {
        public PaymentFailedException(string message) : base(message)
        {
        }
    }
    
    /// <summary>
    /// Exception thrown when email sending fails
    /// </summary>
    public class EmailSendFailedException : Exception
    {
        public EmailSendFailedException(string message) : base(message)
        {
        }
    }
}