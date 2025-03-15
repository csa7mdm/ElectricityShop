using System;

namespace ElectricityShop.Domain.Entities
{
    public class Address : BaseEntity
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
        public bool IsDefault { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}