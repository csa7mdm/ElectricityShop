using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Interface for executing plugin extension points.
    /// </summary>
    /// <typeparam name="TExtensionPoint">The type of extension point to execute.</typeparam>
    public interface IPluginExtensionExecutor<TExtensionPoint>
    {
        /// <summary>
        /// Executes all registered extensions of the specified type, in order of their execution order.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="executionFunc">The function to execute on each extension point.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of results from all extension points.</returns>
        Task<IEnumerable<TResult>> ExecuteAllAsync<TResult>(
            Func<TExtensionPoint, Task<TResult>> executionFunc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes all registered extensions of the specified type, in order of their execution order.
        /// </summary>
        /// <param name="executionAction">The action to execute on each extension point.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteAllAsync(
            Func<TExtensionPoint, Task> executionAction,
            CancellationToken cancellationToken = default);
    }
}