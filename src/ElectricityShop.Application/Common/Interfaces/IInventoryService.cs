using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for inventory services
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Deducts inventory for a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to deduct</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task DeductInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns inventory for a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to return</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task ReturnInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a product has enough inventory
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to check</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>True if the product has enough inventory; otherwise, false</returns>
        Task<bool> HasEnoughInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default);
    }
}