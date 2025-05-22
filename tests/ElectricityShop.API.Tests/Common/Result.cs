using System.Collections.Generic;

namespace ElectricityShop.Application.Common.Models
{
    // Define the Result class for testing purposes
    public class Result
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}