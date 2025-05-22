using System;

namespace ElectricityShop.Application.Features.Products.Queries
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }
}
