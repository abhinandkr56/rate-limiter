namespace SMS_Rate_Limiter.Services;

public interface IRateLimitService
{
    bool CanSend(string accountId, string businessPhone);
}