using SMS_Rate_Limiter.Models;

namespace SMS_Rate_Limiter.Services;

public interface IMongoService
{
    Account? GetAccount(string accountId);
    BusinessNumber? GetBusinessNumber(string accountId, string businessPhone);
    void LogHistory(RateLimitHistory history);
    List<RateLimitHistory> GetHistory(string? accountId, string? businessPhone, DateTime? start, DateTime? end);
    List<Account> GetAllAccounts();
    List<BusinessNumber> GetBusinessNumbers(string accountId);
}