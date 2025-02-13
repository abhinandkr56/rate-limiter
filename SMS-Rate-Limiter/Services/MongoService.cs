using MongoDB.Driver;
using SMS_Rate_Limiter.Models;

namespace SMS_Rate_Limiter.Services;

public class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;

    public MongoService(IMongoClient client)
    {
        _database = client.GetDatabase("rateLimiter");
    }

    public Account GetAccount(string accountId)
    {
        return _database.GetCollection<Account>("accounts")
            .Find(a => a.Id == accountId)
            .FirstOrDefault();
    }

    public BusinessNumber GetBusinessNumber(string accountId, string businessPhone)
    {
        return _database.GetCollection<BusinessNumber>("businessNumbers")
            .Find(b => b.AccountId == accountId && b.PhoneNumber == businessPhone)
            .FirstOrDefault();
    }

    public void LogHistory(RateLimitHistory history)
    {
        _database.GetCollection<RateLimitHistory>("history")
            .InsertOne(history);
    }

    public List<RateLimitHistory> GetHistory(string? accountId, string? businessPhone, DateTime? start, DateTime? end)
    {
        var filters = new List<FilterDefinition<RateLimitHistory>>();
        var builder = Builders<RateLimitHistory>.Filter;

        if (!string.IsNullOrEmpty(accountId))
        {
            filters.Add(builder.Eq(h => h.AccountId, accountId));
        }

        if (!string.IsNullOrEmpty(businessPhone))
        {
            filters.Add(builder.Eq(h => h.BusinessPhone, businessPhone));
        }

        if (start.HasValue)
        {
            filters.Add(builder.Gte(h => h.Timestamp, start.Value));
        }

        if (end.HasValue)
        {
            filters.Add(builder.Lte(h => h.Timestamp, end.Value));
        }

        // Combine filters with AND if filters exist; otherwise, use an empty filter to return all records
        var finalFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

        return _database.GetCollection<RateLimitHistory>("history")
            .Find(finalFilter)
            .ToList();
    }

    public List<Account> GetAllAccounts()
    {
        return _database.GetCollection<Account>("accounts")
            .Find(_ => true).ToList();
    }

    public List<BusinessNumber> GetBusinessNumbers(string accountId)
    {
        return _database.GetCollection<BusinessNumber>("businessNumbers")
            .Find(b => b.AccountId == accountId).ToList();
    }
}