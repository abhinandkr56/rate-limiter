using Microsoft.Extensions.Caching.Memory;
using Moq;
using SMS_Rate_Limiter.Models;
using SMS_Rate_Limiter.Services;

namespace SMS_Rate_Limiter.Test;

public class RateLimitServiceTests
{
    private readonly Mock<IMongoService> _mongoServiceMock;
    private readonly IMemoryCache _cache;
    private readonly RateLimitService _rateLimitService;

    public RateLimitServiceTests()
    {
        _mongoServiceMock = new Mock<IMongoService>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _rateLimitService = new RateLimitService(_cache, _mongoServiceMock.Object);
    }

    [Fact]
    public void CanSend_ReturnsFalse_WhenAccountDoesNotExist()
    {
        // Arrange
        _mongoServiceMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns((Account)null);

        // Act
        var result = _rateLimitService.CanSend("testAccount", "1234567890");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSend_ReturnsFalse_WhenBusinessNumberDoesNotExist()
    {
        // Arrange
        _mongoServiceMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(new Account { GlobalLimit = 10 });
        _mongoServiceMock.Setup(m => m.GetBusinessNumber(It.IsAny<string>(), It.IsAny<string>())).Returns((BusinessNumber)null);

        // Act
        var result = _rateLimitService.CanSend("testAccount", "1234567890");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSend_ReturnsFalse_WhenGlobalLimitExceeded()
    {
        // Arrange
        var account = new Account { GlobalLimit = 1 };
        _mongoServiceMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
        _mongoServiceMock.Setup(m => m.GetBusinessNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(new BusinessNumber { PerSecondLimit = 10 });
        
        _cache.Set("testAccount_global", 1);
        
        // Act
        var result = _rateLimitService.CanSend("testAccount", "1234567890");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSend_ReturnsFalse_WhenBusinessLimitExceeded()
    {
        // Arrange
        var account = new Account { GlobalLimit = 10 };
        var businessNumber = new BusinessNumber { PerSecondLimit = 1 };
        
        _mongoServiceMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
        _mongoServiceMock.Setup(m => m.GetBusinessNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(businessNumber);
        
        _cache.Set("testAccount_1234567890", 1);
        
        // Act
        var result = _rateLimitService.CanSend("testAccount", "1234567890");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSend_ReturnsTrue_WhenLimitsAreNotExceeded()
    {
        // Arrange
        var account = new Account { GlobalLimit = 10 };
        var businessNumber = new BusinessNumber { PerSecondLimit = 5 };
        
        _mongoServiceMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
        _mongoServiceMock.Setup(m => m.GetBusinessNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(businessNumber);
        
        _mongoServiceMock.Setup(m => m.LogHistory(It.IsAny<RateLimitHistory>()));

        // Act
        var result = _rateLimitService.CanSend("testAccount", "1234567890");

        // Assert
        Assert.True(result);
    }
}