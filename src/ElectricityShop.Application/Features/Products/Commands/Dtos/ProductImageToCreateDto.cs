namespace ElectricityShop.Application.Features.Products.Commands.Dtos
{
    public class ProductImageToCreateDto
    {
        public required string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
