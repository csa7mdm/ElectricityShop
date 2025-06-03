using ElectricityShop.Application.Features.Products.Commands;
using ElectricityShop.Application.Features.Products.Commands.Handlers;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Products.Commands
{
    public class UpdateProductCommandTests
    {
        private readonly Mock<IRepository<Product>> _productRepositoryMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;
        private readonly UpdateProductCommandHandler _handler;

        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();
        private readonly Guid _newCategoryId = Guid.NewGuid();

        public UpdateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();

            _handler = new UpdateProductCommandHandler(
                _loggerMock.Object,
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesProduct()
        {
            // Arrange
            Product product = new Product
            {
                Id = _productId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 9.99m,
                StockQuantity = 10,
                CategoryId = _categoryId,
                IsActive = true
            };

            Category category = new Category { Id = _categoryId, Name = "Old Category", Description = "Desc" };
            Category newCategory = new Category { Id = _newCategoryId, Name = "New Category", Description = "Desc" };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_productId))
                .ReturnsAsync(product);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(_newCategoryId))
                .ReturnsAsync(newCategory);

            var command = new UpdateProductCommand
            {
                Id = _productId,
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = _newCategoryId,
                IsActive = true
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            // Verify product was updated
            Assert.Equal("New Name", product.Name);
            Assert.Equal("New Description", product.Description);
            Assert.Equal(19.99m, product.Price);
            Assert.Equal(20, product.StockQuantity);
            Assert.Equal(_newCategoryId, product.CategoryId);

            _productRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ReturnsFalse()
        {
            // Arrange
            Product? nullProduct = null;
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => nullProduct);

            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);

            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}