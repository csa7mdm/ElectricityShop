using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.Abstractions.ExtensionPoints
{
    /// <summary>
    /// Represents an extension point for product-related operations.
    /// </summary>
    public interface IProductExtensionPoint
    {
        /// <summary>
        /// Gets the execution order of this extension point. Lower values execute earlier.
        /// </summary>
        int ExecutionOrder { get; }

        /// <summary>
        /// Called before a product is retrieved.
        /// </summary>
        /// <param name="productId">The ID of the product being retrieved.</param>
        /// <param name="context">The context for this operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task OnBeforeProductRetrievedAsync(int productId, IDictionary<string, object> context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Called after a product is retrieved.
        /// </summary>
        /// <param name="productId">The ID of the product that was retrieved.</param>
        /// <param name="context">The context for this operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task OnAfterProductRetrievedAsync(int productId, IDictionary<string, object> context, CancellationToken cancellationToken = default);
    }
}