namespace ElectricityShop.API.Infrastructure.Caching
{
    /// <summary>
    /// Contains constants for cache keys used throughout the application
    /// </summary>
    public static class CacheKeys
    {
        /// <summary>
        /// Key for the product list cache
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <returns>Cache key string</returns>
        public static string ProductsList(int pageNumber, int pageSize, Guid? categoryId = null) =>
            $"products_list_p{pageNumber}_s{pageSize}{(categoryId.HasValue ? $"_c{categoryId}" : "")}";

        /// <summary>
        /// Key for individual product cache
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Cache key string</returns>
        public static string ProductDetails(Guid id) => $"product_{id}";

        /// <summary>
        /// Key for product categories list cache
        /// </summary>
        public static string CategoriesList = "categories_list";
    }
}