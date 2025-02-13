using Microsoft.Extensions.Caching.Memory;
using SMS_Rate_Limiter.Models;

namespace SMS_Rate_Limiter.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IMemoryCache _cache;
    private readonly IMongoService _mongoService;
    private readonly TimeSpan _windowSize = TimeSpan.FromSeconds(5);

    public RateLimitService(IMemoryCache cache, IMongoService mongoService)
    {
        _cache = cache;
        _mongoService = mongoService;
    }

    public bool CanSend(string accountId, string businessPhone)
    {
        var account = _mongoService.GetAccount(accountId);
        if (account == null) return false;

        var businessNumber = _mongoService.GetBusinessNumber(accountId, businessPhone);
        if (businessNumber == null) return false;

        // Check global limit
        var globalKey = $"{accountId}_global";
        if (!CheckLimit(globalKey, account.GlobalLimit)) return false;

        // Check business limit
        var businessKey = $"{accountId}_{businessPhone}";
        if (!CheckLimit(businessKey, businessNumber.PerSecondLimit)) return false;

        // Log allowed attempt
        _mongoService.LogHistory(new RateLimitHistory
        {
            AccountId = accountId,
            BusinessPhone = businessPhone,
            Timestamp = DateTime.UtcNow,
            Allowed = true
        });

        return true;
    }

    private bool CheckLimit(string key, int limit)
    {
        var counter = _cache.GetOrCreate(key, entry =>
        {
            entry.SlidingExpiration = _windowSize;
            return 0;
        });

        if (counter >= limit) return false;
        _cache.Set(key, counter + 1);
        return true;
    }

}