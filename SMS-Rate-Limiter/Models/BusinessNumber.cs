using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SMS_Rate_Limiter.Models;

public class BusinessNumber
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string AccountId { get; set; }
    public string PhoneNumber { get; set; }
    public int PerSecondLimit { get; set; }
}