using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Service for processing payments
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Processes a payment for an order
        /// </summary>
        /// <param name="orderId">ID of the order</param>
        /// <param name="amount">Amount to charge</param>
        /// <param name="paymentMethodId">ID of the payment method to use</param>
        /// <returns>Result of the payment processing</returns>
        Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, Guid paymentMethodId);
        
        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="orderId">ID of the order</param>
        /// <param name="amount">Amount to refund</param>
        /// <param name="transactionId">Original transaction ID</param>
        /// <returns>Result of the refund processing</returns>
        Task<PaymentResult> ProcessRefundAsync(Guid orderId, decimal amount, string transactionId);
    }
    
    /// <summary>
    /// Result of a payment operation
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// Whether the payment was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Transaction ID from the payment provider
        /// </summary>
        public string TransactionId { get; set; }
        
        /// <summary>
        /// Error message if payment failed
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Creates a successful payment result
        /// </summary>
        public static PaymentResult Succeeded(string transactionId) => 
            new PaymentResult { Success = true, TransactionId = transactionId };
        
        /// <summary>
        /// Creates a failed payment result
        /// </summary>
        public static PaymentResult Failed(string errorMessage) => 
            new PaymentResult { Success = false, ErrorMessage = errorMessage };
    }
}