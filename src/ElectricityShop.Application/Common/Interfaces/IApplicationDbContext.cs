using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the application database context
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Gets the products DbSet
        /// </summary>
        DbSet<Product> Products { get; }
        
        /// <summary>
        /// Gets the orders DbSet
        /// </summary>
        DbSet<Order> Orders { get; }
        
        /// <summary>
        /// Gets the order items DbSet
        /// </summary>
        DbSet<OrderItem> OrderItems { get; }
        
        /// <summary>
        /// Gets the failed messages DbSet
        /// </summary>
        DbSet<FailedMessage> FailedMessages { get; }
        
        /// <summary>
        /// Gets a DbSet for the specified entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The DbSet for the specified entity type</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        
        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The number of affected records</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}