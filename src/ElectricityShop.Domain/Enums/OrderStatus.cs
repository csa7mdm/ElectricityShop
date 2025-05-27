namespace ElectricityShop.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        PaymentProcessed,
        PaymentPending,
        Failed
    }
}