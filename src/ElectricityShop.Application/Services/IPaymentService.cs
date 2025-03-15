using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
        Task<PaymentStatus> GetPaymentStatusAsync(string paymentId);
    }

    public class PaymentRequest
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
        public BillingAddress BillingAddress { get; set; }
    }

    public class BillingAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Authorized,
        Captured,
        Failed,
        Refunded,
        Cancelled
    }
}