using ElectricityShop.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace ElectricityShop.API.Tests.Extensions
{
    public class AuthenticationExtensionsTests
    {
        [Fact]
        public void AddAuthorizationPolicies_RegistersAuthorizationServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddAuthorizationPolicies();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            // We can't directly test the policy contents, but we can verify the authorization services were registered
            var authorizationOptions = serviceProvider.GetRequiredService<IAuthorizationService>();
            Assert.NotNull(authorizationOptions);
        }

        [Fact]
        public void AddAuthorizationPolicies_WithExistingAuthorizationServices_DoesNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAuthorization();

            // Act & Assert - No exception should be thrown
            var exception = Record.Exception(() => services.AddAuthorizationPolicies());
            Assert.Null(exception);
        }
    }
}