using MediatR;

namespace ElectricityShop.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Initial stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Indicates if the product is active. Defaults to true.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Collection of images for the product.
        /// </summary>
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        /// <summary>
        /// Collection of attributes for the product.
        /// </summary>
        public ICollection<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
    }
}