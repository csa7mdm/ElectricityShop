using ElectricityShop.Infrastructure.BackgroundJobs;
using Hangfire;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.BackgroundJobs
{
    public class HangfireErrorHandlerTests
    {
        private readonly Mock<ILogger<HangfireErrorHandler>> _loggerMock;
        private readonly HangfireErrorHandler _errorHandler;
        
        public HangfireErrorHandlerTests()
        {
            _loggerMock = new Mock<ILogger<HangfireErrorHandler>>();
            _errorHandler = new HangfireErrorHandler(_loggerMock.Object);
        }
        
        [Fact]
        public void OnPerforming_DoesNothing()
        {
            // Arrange
            var contextMock = new Mock<PerformingContext>();
            
            // Act
            _errorHandler.OnPerforming(contextMock.Object);
            
            // Assert - no exceptions means success
            // Nothing to verify as the method is intentionally empty
        }
        
        [Fact]
        public void OnPerformed_WithException_LogsError()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var backgroundJobMock = new Mock<BackgroundJob>();
            backgroundJobMock.Setup(x => x.Id).Returns("job123");
            
            var jobMock = new Mock<Job>();
            jobMock.Setup(x => x.Type).Returns(typeof(TestJob));
            jobMock.Setup(x => x.Method).Returns(typeof(TestJob).GetMethod("Execute"));
            
            backgroundJobMock.Setup(x => x.Job).Returns(jobMock.Object);
            
            var contextMock = new Mock<PerformedContext>();
            contextMock.Setup(x => x.Exception).Returns(exception);
            contextMock.Setup(x => x.BackgroundJob).Returns(backgroundJobMock.Object);
            
            // Act
            _errorHandler.OnPerformed(contextMock.Object);
            
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.Is<Exception>(ex => ex == exception),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
        
        [Fact]
        public void OnPerformed_WithoutException_DoesNotLog()
        {
            // Arrange
            var contextMock = new Mock<PerformedContext>();
            contextMock.Setup(x => x.Exception).Returns((Exception)null);
            
            // Act
            _errorHandler.OnPerformed(contextMock.Object);
            
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }
        
        [Fact]
        public void OnStateApplied_FailedState_LogsStructuredError()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var failedState = new FailedState(exception);
            
            var backgroundJobMock = new Mock<BackgroundJob>();
            backgroundJobMock.Setup(x => x.Id).Returns("job123");
            
            var jobMock = new Mock<Job>();
            jobMock.Setup(x => x.Type).Returns(typeof(TestJob));
            jobMock.Setup(x => x.Method).Returns(typeof(TestJob).GetMethod("Execute"));
            jobMock.Setup(x => x.Args).Returns(new object[] { "arg1" });
            
            backgroundJobMock.Setup(x => x.Job).Returns(jobMock.Object);
            
            var contextMock = new Mock<ApplyStateContext>();
            contextMock.Setup(x => x.NewState).Returns(failedState);
            contextMock.Setup(x => x.BackgroundJob).Returns(backgroundJobMock.Object);
            
            var transactionMock = new Mock<IWriteOnlyTransaction>();
            
            // Act
            _errorHandler.OnStateApplied(contextMock.Object, transactionMock.Object);
            
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Background job failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        [Fact]
        public void OnStateApplied_NonFailedState_DoesNotLog()
        {
            // Arrange
            var successState = new SucceededState();
            
            var contextMock = new Mock<ApplyStateContext>();
            contextMock.Setup(x => x.NewState).Returns(successState);
            
            var transactionMock = new Mock<IWriteOnlyTransaction>();
            
            // Act
            _errorHandler.OnStateApplied(contextMock.Object, transactionMock.Object);
            
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }
        
        [Fact]
        public void OnStateUnapplied_DoesNothing()
        {
            // Arrange
            var contextMock = new Mock<ApplyStateContext>();
            var transactionMock = new Mock<IWriteOnlyTransaction>();
            
            // Act
            _errorHandler.OnStateUnapplied(contextMock.Object, transactionMock.Object);
            
            // Assert - no exceptions means success
            // Nothing to verify as the method is intentionally empty
        }
        
        // Test job class for the tests
        private class TestJob
        {
            public void Execute(string arg)
            {
                // Test method
            }
        }
    }
}