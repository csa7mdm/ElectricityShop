using System.Collections.Generic;

namespace ElectricityShop.Application.Common.Models
{
    // Define an AuthenticationResult class that matches what's expected
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty; // Initialize with empty string
        public string RefreshToken { get; set; } = string.Empty; // Initialize with empty string
        
        // Add constructor for explicitly setting values
        public AuthenticationResult()
        {
            // Default constructor with properties initialized above
        }
        
        public AuthenticationResult(string token, string refreshToken = "")
        {
            Token = token;
            RefreshToken = refreshToken;
            Success = true;
        }
        
        public AuthenticationResult(List<string> errors)
        {
            Success = false;
            Errors = errors;
        }
    }
}