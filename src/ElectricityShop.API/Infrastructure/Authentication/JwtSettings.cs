namespace ElectricityShop.API.Infrastructure.Authentication
{
    /// <summary>
    /// Settings for JWT token generation and validation
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key used to sign the JWT token
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Issuer of the JWT token
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience of the JWT token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpiryMinutes { get; set; }

        /// <summary>
        /// Refresh token expiration time in days
        /// </summary>
        public int RefreshTokenExpiryDays { get; set; }
    }
}