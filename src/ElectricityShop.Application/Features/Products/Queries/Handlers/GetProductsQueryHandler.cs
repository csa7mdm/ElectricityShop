using ElectricityShop.Application.Common.Extensions;
using ElectricityShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Products.Queries.Handlers
{
    /// <summary>
    /// Handler for GetProductsQuery with optimized database access
    /// </summary>
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        
        // Compiled queries for frequently executed operations
        // Note: We cannot use compile queries with interfaces, so we'll use LINQ directly
        
        public GetProductsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // Build the query using LINQ
            var baseQuery = from p in _dbContext.Products
                          join c in _dbContext.Categories on p.CategoryId equals c.Id
                          where (request.IncludeInactive || p.IsActive) &&
                                (string.IsNullOrEmpty(request.SearchTerm) || 
                                 p.Name.Contains(request.SearchTerm) || 
                                 p.Description.Contains(request.SearchTerm)) &&
                                (!request.MinPrice.HasValue || p.Price >= request.MinPrice.Value) &&
                                (!request.MaxPrice.HasValue || p.Price <= request.MaxPrice.Value) &&
                                (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value)
                          select new ProductDto
                          {
                              Id = p.Id,
                              Name = p.Name,
                              Description = p.Description,
                              Price = p.Price,
                              StockQuantity = p.StockQuantity,
                              CategoryId = p.CategoryId,
                              CategoryName = c.Name,
                              IsActive = p.IsActive
                              // Other properties mapped here...
                          };
            
            // Get the count (using the optimized database access extension)
            int totalCount;
            List<ProductDto> products;
            
            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                // Use ToPaginatedListAsync extension for optimized pagination
                var result = await baseQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
                products = result.Items;
                totalCount = result.TotalCount;
            }
            else
            {
                // If pagination is not requested, get all items
                products = await baseQuery.ToListAsync(cancellationToken);
                totalCount = products.Count;
            }
            
            // Set the total count for the UI to calculate pagination
            request.TotalCount = totalCount;
            
            return products;
        }
    }
}