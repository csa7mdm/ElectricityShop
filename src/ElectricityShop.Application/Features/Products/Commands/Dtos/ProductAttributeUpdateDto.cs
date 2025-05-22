namespace ElectricityShop.Application.Features.Products.Commands.Dtos
{
    // For the simplified "clear and add" approach, this is similar to ProductAttributeToCreateDto
    // If a more complex update (preserving IDs) was needed, this DTO would include an Id.
    public class ProductAttributeUpdateDto
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
