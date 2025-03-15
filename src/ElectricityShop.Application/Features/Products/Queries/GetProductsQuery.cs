using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public string SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetProductsQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
            {
                var products = await _unitOfWork.Products.ListAllAsync();

                // Filter by search term
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    products = products.Where(p => 
                        p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                        p.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filter by category
                if (request.CategoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryId == request.CategoryId.Value).ToList();
                }

                // Filter by price range
                if (request.MinPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= request.MinPrice.Value).ToList();
                }

                if (request.MaxPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= request.MaxPrice.Value).ToList();
                }

                // Apply pagination
                var pagedProducts = products
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Convert to DTOs
                return pagedProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    IsActive = p.IsActive,
                    MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
                }).ToList();
            }
        }
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string MainImageUrl { get; set; }
    }
}