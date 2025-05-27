using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Executes plugin extension points in the correct order.
    /// </summary>
    public class PluginExtensionExecutor<TExtensionPoint> : IPluginExtensionExecutor<TExtensionPoint>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginExtensionExecutor<TExtensionPoint>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginExtensionExecutor{TExtensionPoint}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        public PluginExtensionExecutor(
            IServiceProvider serviceProvider,
            ILogger<PluginExtensionExecutor<TExtensionPoint>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executes all registered extensions of the specified type, in order of their execution order.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="executionFunc">The function to execute on each extension point.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of results from all extension points.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAllAsync<TResult>(
            Func<TExtensionPoint, Task<TResult>> executionFunc,
            CancellationToken cancellationToken = default)
        {
            var results = new List<TResult>();
            var extensions = _serviceProvider.GetServices<TExtensionPoint>();

            // Execute extensions in order of execution order
            var orderedExtensions = extensions;
            
            // If the extension point has an ExecutionOrder property, order by it
            var executionOrderProperty = typeof(TExtensionPoint).GetProperty("ExecutionOrder");
            if (executionOrderProperty != null)
            {
                orderedExtensions = extensions.OrderBy(e => executionOrderProperty.GetValue(e));
            }

            foreach (var extension in orderedExtensions)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = await executionFunc(extension);
                    results.Add(result);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error executing extension {ExtensionType}: {ErrorMessage}",
                        extension.GetType().Name, ex.Message);
                }
            }

            return results;
        }

        /// <summary>
        /// Executes all registered extensions of the specified type, in order of their execution order.
        /// </summary>
        /// <param name="executionAction">The action to execute on each extension point.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAllAsync(
            Func<TExtensionPoint, Task> executionAction,
            CancellationToken cancellationToken = default)
        {
            var extensions = _serviceProvider.GetServices<TExtensionPoint>();

            // Execute extensions in order of execution order
            var orderedExtensions = extensions;
            
            // If the extension point has an ExecutionOrder property, order by it
            var executionOrderProperty = typeof(TExtensionPoint).GetProperty("ExecutionOrder");
            if (executionOrderProperty != null)
            {
                orderedExtensions = extensions.OrderBy(e => executionOrderProperty.GetValue(e));
            }

            foreach (var extension in orderedExtensions)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await executionAction(extension);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error executing extension {ExtensionType}: {ErrorMessage}",
                        extension.GetType().Name, ex.Message);
                }
            }
        }
    }
}