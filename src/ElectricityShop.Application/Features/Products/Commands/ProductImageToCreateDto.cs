namespace ElectricityShop.Application.Features.Products.Commands
{
    public class ProductImageToCreateDto
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
