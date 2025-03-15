using System;
using System.Collections.Generic;
using ElectricityShop.Domain.Enums;

namespace ElectricityShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public required string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public required string ShippingAddress { get; set; }
        public required string BillingAddress { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}