namespace ElectricityShop.Application.Features.Products.Commands.Dtos
{
    // For the simplified "clear and add" approach, this is similar to ProductImageToCreateDto
    // If a more complex update (preserving IDs) was needed, this DTO would include an Id.
    public class ProductImageUpdateDto
    {
        public required string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
