using System;

namespace ElectricityShop.Infrastructure.Messaging.Events
{
    public class OrderPlacedEvent : IntegrationEvent
    {
        public Guid OrderId { get; }
        public string OrderNumber { get; }
        public Guid UserId { get; }
        public decimal TotalAmount { get; }
        public string PaymentMethod { get; }

        public OrderPlacedEvent(Guid orderId, string orderNumber, Guid userId, decimal totalAmount, string paymentMethod)
        {
            OrderId = orderId;
            OrderNumber = orderNumber;
            UserId = userId;
            TotalAmount = totalAmount;
            PaymentMethod = paymentMethod;
        }
    }
}