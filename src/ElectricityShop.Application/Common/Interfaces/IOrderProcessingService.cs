using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Service for processing orders asynchronously
    /// </summary>
    public interface IOrderProcessingService
    {
        /// <summary>
        /// Processes payment for an order
        /// </summary>
        Task ProcessPaymentAsync(Guid orderId);
        
        /// <summary>
        /// Sends order confirmation email
        /// </summary>
        Task SendOrderConfirmationEmailAsync(Guid orderId);
        
        /// <summary>
        /// Notifies inventory system to update stock
        /// </summary>
        Task UpdateInventoryAsync(Guid orderId);
        
        /// <summary>
        /// Completes order processing
        /// </summary>
        Task FinalizeOrderAsync(Guid orderId);
    }
}