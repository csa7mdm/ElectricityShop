using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElectricityShop.Application.Common.Extensions
{
    /// <summary>
    /// Extensions for IQueryable to optimize database access
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Applies pagination to a query
        /// </summary>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Gets a paginated list from a query
        /// </summary>
        public static async Task<(List<T> Items, int TotalCount)> ToPaginatedListAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.ApplyPaging(pageNumber, pageSize).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Applies an optional filter to a query
        /// </summary>
        public static IQueryable<T> ApplyOptionalFilter<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// Applies an optional ordering to a query
        /// </summary>
        public static IQueryable<T> ApplyOptionalOrderBy<T, TKey>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, TKey>> keySelector,
            bool descending = false)
        {
            if (!condition)
                return query;

            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        /// <summary>
        /// Creates a compiled query for better performance on frequently executed queries
        /// </summary>
        public static Func<TContext, TParam, IQueryable<TResult>> CreateCompiledQuery<TContext, TParam, TResult>(
            Expression<Func<TContext, TParam, IQueryable<TResult>>> queryExpression)
            where TContext : DbContext
        {
            return (Func<TContext, TParam, IQueryable<TResult>>)EF.CompileQuery(queryExpression);
        }

        /// <summary>
        /// Creates a compiled count query for better performance
        /// </summary>
        public static Func<TContext, TParam, int> CreateCompiledCountQuery<TContext, TParam, TEntity>(
            Expression<Func<TContext, TParam, IQueryable<TEntity>>> queryExpression)
            where TContext : DbContext
        {
            var countExpression = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Count),
                new[] { typeof(TEntity) },
                queryExpression.Body);

            var lambdaExpression = Expression.Lambda<Func<TContext, TParam, int>>(
                countExpression,
                queryExpression.Parameters);

            return EF.CompileQuery(lambdaExpression);
        }
    }
}