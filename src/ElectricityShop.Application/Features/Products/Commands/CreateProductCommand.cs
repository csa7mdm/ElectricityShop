using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        public class ProductAttributeDto
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ProductImageDto
        {
            public string ImageUrl { get; set; }
            public bool IsMain { get; set; }
        }

        public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreateProductCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                // Add attributes
                foreach (var attr in request.Attributes)
                {
                    var productAttribute = new ProductAttribute
                    {
                        Id = Guid.NewGuid(),
                        Name = attr.Name,
                        Value = attr.Value,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.ProductAttributes.AddAsync(productAttribute);
                }

                // Add images
                foreach (var img in request.Images)
                {
                    var productImage = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = img.ImageUrl,
                        IsMain = img.IsMain,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.ProductImages.AddAsync(productImage);
                }

                await _unitOfWork.CompleteAsync();

                return product.Id;
            }
        }
    }
}