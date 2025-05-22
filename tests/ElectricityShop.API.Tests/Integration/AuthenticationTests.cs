using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using ElectricityShop.API.Controllers;
using System.Net;

namespace ElectricityShop.API.Tests.Integration
{
    public class AuthenticationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "integration_test@example.com",
                Password = "Password123!",
                FirstName = "Integration",
                LastName = "Test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.True((bool)content.success);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var refreshRequest = new RefreshTokenRequest
            {
                Token = "invalid_token",
                RefreshToken = "invalid_refresh_token"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}