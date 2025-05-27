using ElectricityShop.API.Controllers;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _controller = new AuthController(_mockIdentityService.Object);
        }

        [Fact]
        public async Task Register_Success_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            var result = new AuthenticationResult { Success = true };
            _mockIdentityService.Setup(x => x.RegisterAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(result));

            // Act
            var actionResult = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Same(result, okResult.Value);

            _mockIdentityService.Verify(x => x.RegisterAsync(
                request.Email, request.Password, request.FirstName, request.LastName), Times.Once);
        }

        [Fact]
        public async Task Register_Failure_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "invalid-email",
                Password = "weak",
                FirstName = "Test",
                LastName = "User"
            };

            var errors = new List<string> { "Invalid email format", "Password too weak" };
            _mockIdentityService.Setup(x => x.RegisterAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationResult { Success = false, Errors = errors }));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic returnValue = badRequestResult.Value;
            Assert.Equal(errors, returnValue.Errors);

            _mockIdentityService.Verify(x => x.RegisterAsync(
                request.Email, request.Password, request.FirstName, request.LastName), Times.Once);
        }

        [Fact]
        public async Task Login_Success_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var authResult = new AuthenticationResult { Success = true };
            _mockIdentityService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(authResult));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(authResult, okResult.Value);

            _mockIdentityService.Verify(x => x.LoginAsync(request.Email, request.Password), Times.Once);
        }

        [Fact]
        public async Task Login_Failure_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var errors = new List<string> { "Invalid credentials" };
            _mockIdentityService.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationResult { Success = false, Errors = errors }));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic returnValue = badRequestResult.Value;
            Assert.Equal(errors, returnValue.Errors);

            _mockIdentityService.Verify(x => x.LoginAsync(request.Email, request.Password), Times.Once);
        }

        [Fact]
        public async Task Refresh_Success_ReturnsOkResult()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                Token = "expired-token",
                RefreshToken = "valid-refresh-token"
            };

            var authResult = new AuthenticationResult { Success = true };
            _mockIdentityService.Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(authResult));

            // Act
            var result = await _controller.Refresh(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(authResult, okResult.Value);

            _mockIdentityService.Verify(x => x.RefreshTokenAsync(request.Token, request.RefreshToken), Times.Once);
        }

        [Fact]
        public async Task Refresh_Failure_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                Token = "expired-token",
                RefreshToken = "invalid-refresh-token"
            };

            var errors = new List<string> { "Invalid refresh token" };
            _mockIdentityService.Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationResult { Success = false, Errors = errors }));

            // Act
            var result = await _controller.Refresh(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic returnValue = badRequestResult.Value;
            Assert.Equal(errors, returnValue.Errors);

            _mockIdentityService.Verify(x => x.RefreshTokenAsync(request.Token, request.RefreshToken), Times.Once);
        }
    }
}