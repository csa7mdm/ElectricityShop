using ElectricityShop.Application.Common.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// SMTP implementation of email service using FluentEmail
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly IFluentEmail _emailSender;
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;
        
        /// <summary>
        /// Initializes a new instance of the SmtpEmailService
        /// </summary>
        public SmtpEmailService(
            IFluentEmail emailSender,
            IOptions<EmailSettings> options,
            ILogger<SmtpEmailService> logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Sends order confirmation email
        /// </summary>
        public async Task<EmailResult> SendOrderConfirmationAsync(
            string recipientEmail, 
            string recipientName, 
            string orderNumber, 
            List<OrderItemEmailModel> items, 
            decimal totalAmount)
        {
            try
            {
                _logger.LogInformation("Sending order confirmation email for order {OrderNumber} to {RecipientEmail}", 
                    orderNumber, recipientEmail);
                
                var email = _emailSender
                    .To(recipientEmail, recipientName)
                    .Subject($"Your Order Confirmation #{orderNumber}")
                    .UsingTemplate(_settings.OrderConfirmationTemplate, new
                    {
                        CustomerName = recipientName,
                        OrderNumber = orderNumber,
                        OrderDate = DateTime.UtcNow.ToString("D"),
                        Items = items,
                        Subtotal = items.Sum(i => i.TotalPrice),
                        Tax = Math.Round(totalAmount * 0.1m, 2), // Example tax calculation
                        ShippingCost = Math.Round(totalAmount * 0.05m, 2), // Example shipping calculation
                        TotalAmount = totalAmount,
                        StoreAddress = _settings.StoreAddress,
                        StoreName = _settings.StoreName,
                        SupportEmail = _settings.SupportEmail
                    });
                
                var result = await email.SendAsync();
                
                if (result.Successful)
                {
                    _logger.LogInformation("Order confirmation email sent successfully for order {OrderNumber}", orderNumber);
                    return new EmailResult { Sent = true };
                }
                else
                {
                    var errorMessage = string.Join("; ", result.ErrorMessages);
                    _logger.LogWarning("Failed to send order confirmation email for order {OrderNumber}: {ErrorMessage}",
                        orderNumber, errorMessage);
                    return new EmailResult { Sent = false, ErrorMessage = errorMessage };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order confirmation email for order {OrderNumber}", orderNumber);
                return new EmailResult { Sent = false, ErrorMessage = ex.Message };
            }
        }
        
        /// <summary>
        /// Sends payment failed notification email
        /// </summary>
        public async Task<EmailResult> SendPaymentFailedAsync(
            string recipientEmail, 
            string recipientName, 
            string orderNumber, 
            string errorMessage)
        {
            try
            {
                _logger.LogInformation("Sending payment failed email for order {OrderNumber} to {RecipientEmail}", 
                    orderNumber, recipientEmail);
                
                var email = _emailSender
                    .To(recipientEmail, recipientName)
                    .Subject($"Payment Failed for Order #{orderNumber}")
                    .UsingTemplate(_settings.PaymentFailedTemplate, new
                    {
                        CustomerName = recipientName,
                        OrderNumber = orderNumber,
                        ErrorMessage = errorMessage,
                        SupportEmail = _settings.SupportEmail,
                        StoreName = _settings.StoreName
                    });
                
                var result = await email.SendAsync();
                
                if (result.Successful)
                {
                    _logger.LogInformation("Payment failed email sent successfully for order {OrderNumber}", orderNumber);
                    return new EmailResult { Sent = true };
                }
                else
                {
                    var error = string.Join("; ", result.ErrorMessages);
                    _logger.LogWarning("Failed to send payment failed email for order {OrderNumber}: {ErrorMessage}",
                        orderNumber, error);
                    return new EmailResult { Sent = false, ErrorMessage = error };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment failed email for order {OrderNumber}", orderNumber);
                return new EmailResult { Sent = false, ErrorMessage = ex.Message };
            }
        }
    }
    
    /// <summary>
    /// Settings for email service
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Sender email address
        /// </summary>
        public string SenderEmail { get; set; }
        
        /// <summary>
        /// Sender display name
        /// </summary>
        public string SenderName { get; set; }
        
        /// <summary>
        /// Store physical address
        /// </summary>
        public string StoreAddress { get; set; }
        
        /// <summary>
        /// Store name
        /// </summary>
        public string StoreName { get; set; }
        
        /// <summary>
        /// Support email address
        /// </summary>
        public string SupportEmail { get; set; }
        
        /// <summary>
        /// Path to order confirmation email template
        /// </summary>
        public string OrderConfirmationTemplate { get; set; }
        
        /// <summary>
        /// Path to payment failed email template
        /// </summary>
        public string PaymentFailedTemplate { get; set; }
    }
}