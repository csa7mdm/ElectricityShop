namespace ElectricityShop.Application.Features.Products.Commands
{
    public class ProductImageDto
    {
        public required string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }

    public class ProductAttributeDto
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
