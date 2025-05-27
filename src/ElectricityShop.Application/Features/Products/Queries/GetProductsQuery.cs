using MediatR;
using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Products.Queries
{
    /// <summary>
    /// Query to get a paginated list of products
    /// </summary>
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        /// <summary>
        /// Optional category ID to filter products by
        /// </summary>
        public Guid? CategoryId { get; set; }
        
        /// <summary>
        /// Search term to filter products by name or description
        /// </summary>
        public string SearchTerm { get; set; }
        
        /// <summary>
        /// Minimum price filter
        /// </summary>
        public decimal? MinPrice { get; set; }
        
        /// <summary>
        /// Maximum price filter
        /// </summary>
        public decimal? MaxPrice { get; set; }
        
        /// <summary>
        /// Whether to include inactive products (admin only)
        /// </summary>
        public bool IncludeInactive { get; set; }
        
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;
        
        /// <summary>
        /// Total count of items (populated by handler)
        /// </summary>
        public int TotalCount { get; set; }
    }
}