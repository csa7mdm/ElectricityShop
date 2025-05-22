using System;
using System.Collections.Generic;

namespace ElectricityShop.API.Infrastructure.Authentication
{
    /// <summary>
    /// Interface for JWT token generation
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        string GenerateToken(Guid userId, string email, IEnumerable<string> roles);

        /// <summary>
        /// Generates a refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Gets the expiration date for a refresh token
        /// </summary>
        DateTime GetRefreshTokenExpiryTime();
    }
}