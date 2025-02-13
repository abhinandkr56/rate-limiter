using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SMS_Rate_Limiter.Models;

public class RateLimitHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string AccountId { get; set; }
    
    public string AccountName { get; set; }
    public string BusinessPhone { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Allowed { get; set; }
}