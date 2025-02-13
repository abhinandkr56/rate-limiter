using MongoDB.Driver;
using Moq;
using SMS_Rate_Limiter.Models;
using SMS_Rate_Limiter.Services;

namespace SMS_Rate_Limiter.Test;

public class MongoServiceTests
{
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly Mock<IMongoDatabase> _databaseMock;
    private readonly Mock<IMongoCollection<Account>> _accountCollectionMock;
    private readonly Mock<IMongoCollection<BusinessNumber>> _businessNumberCollectionMock;
    private readonly Mock<IMongoCollection<RateLimitHistory>> _historyCollectionMock;
    private readonly MongoService _mongoService;

    public MongoServiceTests()
    {
        _mongoClientMock = new Mock<IMongoClient>();
        _databaseMock = new Mock<IMongoDatabase>();
        _accountCollectionMock = new Mock<IMongoCollection<Account>>();
        _businessNumberCollectionMock = new Mock<IMongoCollection<BusinessNumber>>();
        _historyCollectionMock = new Mock<IMongoCollection<RateLimitHistory>>();

        _mongoClientMock.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(_databaseMock.Object);
        _databaseMock.Setup(d => d.GetCollection<Account>("accounts", null)).Returns(_accountCollectionMock.Object);
        _databaseMock.Setup(d => d.GetCollection<BusinessNumber>("businessNumbers", null)).Returns(_businessNumberCollectionMock.Object);
        _databaseMock.Setup(d => d.GetCollection<RateLimitHistory>("history", null)).Returns(_historyCollectionMock.Object);

        _mongoService = new MongoService(_mongoClientMock.Object);
    }

    [Fact]
    public void GetAccount_ReturnsAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account { Id = "testAccount" };
        var mockCursor = new Mock<IAsyncCursor<Account>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupGet(c => c.Current).Returns(new List<Account> { account });

        _accountCollectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<Account>>(), It.IsAny<FindOptions<Account, Account>>(), default))
            .Returns(mockCursor.Object);

        // Act
        var result = _mongoService.GetAccount("testAccount");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testAccount", result.Id);
    }

    [Fact]
    public void GetBusinessNumber_ReturnsBusinessNumber_WhenExists()
    {
        // Arrange
        var businessNumber = new BusinessNumber { AccountId = "testAccount", PhoneNumber = "1234567890" };
        var mockCursor = new Mock<IAsyncCursor<BusinessNumber>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupGet(c => c.Current).Returns(new List<BusinessNumber> { businessNumber });

        _businessNumberCollectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<BusinessNumber>>(), It.IsAny<FindOptions<BusinessNumber, BusinessNumber>>(), default))
            .Returns(mockCursor.Object);

        // Act
        var result = _mongoService.GetBusinessNumber("testAccount", "1234567890");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1234567890", result.PhoneNumber);
    }

    [Fact]
    public void LogHistory_InsertsDocument()
    {
        // Arrange
        var history = new RateLimitHistory { AccountId = "testAccount", BusinessPhone = "1234567890", Timestamp = DateTime.UtcNow, Allowed = true };

        // Act
        _mongoService.LogHistory(history);

        // Assert
        _historyCollectionMock.Verify(c => c.InsertOne(history, null, default), Times.Once);
    }

    [Fact]
    public void GetAllAccounts_ReturnsListOfAccounts()
    {
        // Arrange
        var accounts = new List<Account> { new Account { Id = "account1" }, new Account { Id = "account2" } };
        var mockCursor = new Mock<IAsyncCursor<Account>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupGet(c => c.Current).Returns(accounts);

        _accountCollectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<Account>>(), It.IsAny<FindOptions<Account, Account>>(), default))
            .Returns(mockCursor.Object);

        // Act
        var result = _mongoService.GetAllAccounts();

        // Assert
        Assert.Equal(2, result.Count);
    }
}