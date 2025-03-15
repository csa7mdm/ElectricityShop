using System;

namespace ElectricityShop.Infrastructure.Identity.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}