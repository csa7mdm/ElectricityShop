using ElectricityShop.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// Implementation of payment service using Stripe
    /// </summary>
    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentSettings _settings;
        private readonly ILogger<StripePaymentService> _logger;
        
        /// <summary>
        /// Initializes a new instance of the StripePaymentService
        /// </summary>
        public StripePaymentService(
            IOptions<PaymentSettings> options,
            ILogger<StripePaymentService> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Processes a payment for an order
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, Guid paymentMethodId)
        {
            try
            {
                _logger.LogInformation("Processing payment of {Amount} for order {OrderId} with method {PaymentMethodId}",
                    amount, orderId, paymentMethodId);
                
                // This is a simplified example without actual Stripe integration
                // In a real application, you would use the Stripe SDK to process the payment
                
                // Simulate API call delay
                await Task.Delay(500);
                
                // For demo purposes, payments with amounts ending in .99 will fail
                if (Math.Round(amount * 100) % 100 == 99)
                {
                    _logger.LogWarning("Payment for order {OrderId} failed: Test failure condition", orderId);
                    return PaymentResult.Failed("Payment declined by processor (test failure)");
                }
                
                // Create transaction ID like what Stripe would provide
                string transactionId = $"tx_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}";
                
                _logger.LogInformation("Payment for order {OrderId} processed successfully with transaction ID {TransactionId}",
                    orderId, transactionId);
                
                return PaymentResult.Succeeded(transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                return PaymentResult.Failed($"Payment processing error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Processes a refund for an order
        /// </summary>
        public async Task<PaymentResult> ProcessRefundAsync(Guid orderId, decimal amount, string transactionId)
        {
            try
            {
                _logger.LogInformation("Processing refund of {Amount} for order {OrderId}, transaction {TransactionId}",
                    amount, orderId, transactionId);
                
                // Simulate API call delay
                await Task.Delay(300);
                
                // For demo purposes, refunds with amounts over 1000 will fail
                if (amount > 1000)
                {
                    _logger.LogWarning("Refund for order {OrderId} failed: Amount exceeds limit", orderId);
                    return PaymentResult.Failed("Refund amount exceeds maximum allowed");
                }
                
                // Create refund transaction ID
                string refundTransactionId = $"re_{transactionId}";
                
                _logger.LogInformation("Refund for order {OrderId} processed successfully with refund ID {RefundTransactionId}",
                    orderId, refundTransactionId);
                
                return PaymentResult.Succeeded(refundTransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for order {OrderId}", orderId);
                return PaymentResult.Failed($"Refund processing error: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Settings for payment processing
    /// </summary>
    public class PaymentSettings
    {
        /// <summary>
        /// API key for the payment provider
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// Secret key for the payment provider
        /// </summary>
        public string SecretKey { get; set; }
        
        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool TestMode { get; set; }
        
        /// <summary>
        /// Currency to use for payments
        /// </summary>
        public string Currency { get; set; } = "USD";
    }
}