using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for notification services
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends an order confirmation notification to the customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="orderId">The order ID</param>
        /// <param name="totalAmount">The total order amount</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendOrderConfirmationAsync(
            Guid customerId, 
            Guid orderId, 
            decimal totalAmount, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an order status change notification to the customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="orderId">The order ID</param>
        /// <param name="newStatus">The new order status</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendOrderStatusChangedAsync(
            Guid customerId, 
            Guid orderId, 
            string newStatus, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an order cancellation notification to the customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="orderId">The order ID</param>
        /// <param name="reason">The cancellation reason</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendOrderCancelledAsync(
            Guid customerId, 
            Guid orderId, 
            string reason, 
            CancellationToken cancellationToken = default);
    }
}