using ElectricityShop.Infrastructure.Caching;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.Caching
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly Mock<IOptionsMonitor<RedisCacheSettings>> _optionsMock;
        private readonly CacheStatistics _statistics;
        private readonly RedisCacheService _cacheService;
        
        public RedisCacheServiceTests()
        {
            // Setup mocks
            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            _optionsMock = new Mock<IOptionsMonitor<RedisCacheSettings>>();
            _statistics = new CacheStatistics();
            
            // Setup redis connection multiplexer
            _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_databaseMock.Object);
                
            // Setup options
            _optionsMock.Setup(x => x.CurrentValue)
                .Returns(new RedisCacheSettings 
                { 
                    ConnectionString = "localhost:6379",
                    InstanceName = "test:",
                    DefaultExpiryMinutes = 30
                });
                
            // Create service instance
            _cacheService = new RedisCacheService(_redisMock.Object, _optionsMock, _statistics);
        }
        
        [Fact]
        public async Task GetAsync_CacheHit_ReturnsDeserializedObject()
        {
            // Arrange
            var testObject = new TestCacheObject { Id = 1, Name = "Test" };
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(testObject);
            
            _databaseMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(serialized);
                
            // Act
            var result = await _cacheService.GetAsync<TestCacheObject>("testkey");
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
            _databaseMock.Verify(x => x.StringGetAsync(It.Is<RedisKey>(k => k == "test:testkey"), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Fact]
        public async Task GetAsync_CacheMiss_ReturnsNull()
        {
            // Arrange
            _databaseMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);
                
            // Act
            var result = await _cacheService.GetAsync<TestCacheObject>("testkey");
            
            // Assert
            Assert.Null(result);
            _databaseMock.Verify(x => x.StringGetAsync(It.Is<RedisKey>(k => k == "test:testkey"), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Fact]
        public async Task SetAsync_CallsStringSet_WithCorrectParameters()
        {
            // Arrange
            var testObject = new TestCacheObject { Id = 1, Name = "Test" };
            var expiry = TimeSpan.FromMinutes(15);
            
            _databaseMock.Setup(x => x.StringSetAsync(
                It.IsAny<RedisKey>(), 
                It.IsAny<RedisValue>(), 
                It.IsAny<TimeSpan?>(), 
                It.IsAny<When>(), 
                It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
                
            // Act
            await _cacheService.SetAsync("testkey", testObject, expiry);
            
            // Assert
            _databaseMock.Verify(x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == "test:testkey"),
                It.IsAny<RedisValue>(),
                It.Is<TimeSpan?>(t => t == expiry),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task RemoveAsync_CallsKeyDelete_WithCorrectKey()
        {
            // Arrange
            _databaseMock.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
                
            // Act
            await _cacheService.RemoveAsync("testkey");
            
            // Assert
            _databaseMock.Verify(x => x.KeyDeleteAsync(It.Is<RedisKey>(k => k == "test:testkey"), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        [Fact]
        public async Task ExistsAsync_CallsKeyExists_ReturnsResult()
        {
            // Arrange
            _databaseMock.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
                
            // Act
            var result = await _cacheService.ExistsAsync("testkey");
            
            // Assert
            Assert.True(result);
            _databaseMock.Verify(x => x.KeyExistsAsync(It.Is<RedisKey>(k => k == "test:testkey"), It.IsAny<CommandFlags>()), Times.Once);
        }
        
        // Test helper class
        private class TestCacheObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}