using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // Added for SqlException

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// Service providing standardized retry policies for the application
    /// </summary>
    public class RetryPolicyService
    {
        private readonly ILogger<RetryPolicyService> _logger;
        
        /// <summary>
        /// Initializes a new instance of the RetryPolicyService
        /// </summary>
        public RetryPolicyService(ILogger<RetryPolicyService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Creates a standard retry policy for database operations
        /// </summary>
        public AsyncRetryPolicy GetDatabaseRetryPolicy()
        {
            return Policy
                .Handle<Microsoft.EntityFrameworkCore.DbUpdateException>()
                .Or<SqlException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Database operation failed. Retry {RetryCount} after {TimeSpan}.",
                            retryCount, timeSpan);
                    });
        }
        
        /// <summary>
        /// Creates a standard retry policy for HTTP operations
        /// </summary>
        public AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (outcome, timeSpan, retryCount, context) =>
                    {
                        if (outcome.Exception != null)
                        {
                            _logger.LogWarning(outcome.Exception,
                                "HTTP request failed. Retry {RetryCount} after {TimeSpan}.",
                                retryCount, timeSpan);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "HTTP request returned {StatusCode}. Retry {RetryCount} after {TimeSpan}.",
                                outcome.Result.StatusCode, retryCount, timeSpan);
                        }
                    });
        }
        
        /// <summary>
        /// Creates a standard retry policy for email operations
        /// </summary>
        public AsyncRetryPolicy GetEmailRetryPolicy()
        {
            return Policy
                .Handle<System.Net.Mail.SmtpException>()
                .Or<Exception>(ex => ex.Message.Contains("email", StringComparison.OrdinalIgnoreCase) &&
                                    IsTransientException(ex))
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Email sending failed. Retry {RetryCount} after {TimeSpan}.",
                            retryCount, timeSpan);
                    });
        }
        
        /// <summary>
        /// Creates a standard retry policy for payment operations
        /// </summary>
        public AsyncRetryPolicy GetPaymentRetryPolicy()
        {
            return Policy
                .Handle<Exception>(ex => IsTransientPaymentException(ex))
                .WaitAndRetryAsync(
                    2, // Fewer retries for payment operations to avoid double charges
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Payment operation failed. Retry {RetryCount} after {TimeSpan}.",
                            retryCount, timeSpan);
                    });
        }
        
        /// <summary>
        /// Creates a generic retry policy for transient errors
        /// </summary>
        public AsyncRetryPolicy GetGenericRetryPolicy()
        {
            return Policy
                .Handle<Exception>(ex => IsTransientException(ex))
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Operation failed with transient error. Retry {RetryCount} after {TimeSpan}.",
                            retryCount, timeSpan);
                    });
        }
        
        private bool IsTransientPaymentException(Exception ex)
        {
            // Customize based on your payment provider's exceptions
            // Only retry on connectivity issues, not on validation/business rule errors
            return ex is TimeoutException ||
                   ex is System.Net.Sockets.SocketException ||
                   ex is HttpRequestException ||
                   (ex.Message?.Contains("timeout", StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (ex.InnerException != null && IsTransientPaymentException(ex.InnerException));
        }
        
        private bool IsTransientException(Exception ex)
        {
            // Determine if exception is transient (can be retried)
            // Examples: temporary network issues, database timeouts
            return ex is TimeoutException ||
                   ex is System.Net.Sockets.SocketException ||
                   ex is System.Net.Http.HttpRequestException ||
                   ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                   (ex.InnerException != null && IsTransientException(ex.InnerException));
        }
    }
}