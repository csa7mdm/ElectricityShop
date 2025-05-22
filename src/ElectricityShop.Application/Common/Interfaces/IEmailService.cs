using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Service for sending emails from the application
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends order confirmation email
        /// </summary>
        Task<EmailResult> SendOrderConfirmationAsync(
            string recipientEmail, 
            string recipientName,
            string orderNumber, 
            List<OrderItemEmailModel> items,
            decimal totalAmount);
            
        /// <summary>
        /// Sends payment failed notification email
        /// </summary>
        Task<EmailResult> SendPaymentFailedAsync(
            string recipientEmail,
            string recipientName,
            string orderNumber,
            string errorMessage);
    }
    
    /// <summary>
    /// Result of email sending operation
    /// </summary>
    public class EmailResult
    {
        /// <summary>
        /// Whether the email was sent successfully
        /// </summary>
        public bool Sent { get; set; }
        
        /// <summary>
        /// Error message if sending failed
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    
    /// <summary>
    /// Model for order item in email templates
    /// </summary>
    public class OrderItemEmailModel
    {
        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Quantity ordered
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Price per unit
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Total price for this item
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}