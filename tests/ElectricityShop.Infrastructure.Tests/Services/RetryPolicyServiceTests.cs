using ElectricityShop.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.Services
{
    public class RetryPolicyServiceTests
    {
        private readonly Mock<ILogger<RetryPolicyService>> _loggerMock;
        private readonly RetryPolicyService _retryPolicyService;
        
        public RetryPolicyServiceTests()
        {
            _loggerMock = new Mock<ILogger<RetryPolicyService>>();
            _retryPolicyService = new RetryPolicyService(_loggerMock.Object);
        }
        
        [Fact]
        public void GetDatabaseRetryPolicy_ReturnsNonNullPolicy()
        {
            // Act
            var policy = _retryPolicyService.GetDatabaseRetryPolicy();
            
            // Assert
            Assert.NotNull(policy);
        }
        
        [Fact]
        public void GetHttpRetryPolicy_ReturnsNonNullPolicy()
        {
            // Act
            var policy = _retryPolicyService.GetHttpRetryPolicy();
            
            // Assert
            Assert.NotNull(policy);
        }
        
        [Fact]
        public void GetEmailRetryPolicy_ReturnsNonNullPolicy()
        {
            // Act
            var policy = _retryPolicyService.GetEmailRetryPolicy();
            
            // Assert
            Assert.NotNull(policy);
        }
        
        [Fact]
        public void GetPaymentRetryPolicy_ReturnsNonNullPolicy()
        {
            // Act
            var policy = _retryPolicyService.GetPaymentRetryPolicy();
            
            // Assert
            Assert.NotNull(policy);
        }
        
        [Fact]
        public void GetGenericRetryPolicy_ReturnsNonNullPolicy()
        {
            // Act
            var policy = _retryPolicyService.GetGenericRetryPolicy();
            
            // Assert
            Assert.NotNull(policy);
        }
        
        [Fact]
        public async Task GetHttpRetryPolicy_RetryOnServerError()
        {
            // Arrange
            var policy = _retryPolicyService.GetHttpRetryPolicy();
            int executionCount = 0;
            
            // Act & Assert
            // This simulates an HTTP request that fails with a server error (500)
            // The policy should retry, and we'll count the number of executions
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await policy.ExecuteAsync(async () =>
                {
                    executionCount++;
                    
                    if (executionCount <= 3) // Allow 3 retries
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                    
                    throw new Exception("Max retries reached");
                });
            });
            
            // The initial attempt plus 3 retries
            Assert.Equal(4, executionCount);
        }
        
        [Fact]
        public async Task GetGenericRetryPolicy_RetryOnTransientException()
        {
            // Arrange
            var policy = _retryPolicyService.GetGenericRetryPolicy();
            int executionCount = 0;
            
            // Act & Assert
            // This simulates an operation that fails with a timeout exception
            // The policy should retry, and we'll count the number of executions
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await policy.ExecuteAsync(async () =>
                {
                    executionCount++;
                    throw new TimeoutException("Simulated timeout");
                });
            });
            
            // The initial attempt plus 3 retries (default retry count)
            Assert.Equal(4, executionCount);
        }
    }
}