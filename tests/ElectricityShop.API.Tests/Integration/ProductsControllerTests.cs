using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using System.Collections.Generic;
using System;

namespace ElectricityShop.API.Tests.Integration
{
    public class ProductsControllerIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsNotImplemented()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/products/{productId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var newProduct = new
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                StockQuantity = 10,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // This test assumes we have a way to authenticate as admin
        // which would require more setup that we're not implementing here
        /*
        [Fact]
        public async Task CreateProduct_AsAdmin_ReturnsCreated()
        {
            // Arrange - would require authentication setup
            // Act
            // Assert
        }
        */

        [Fact]
        public async Task UpdateProduct_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updatedProduct = new
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 129.99m,
                StockQuantity = 5,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updatedProduct);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/products/{productId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}