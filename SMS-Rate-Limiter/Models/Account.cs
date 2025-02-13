using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SMS_Rate_Limiter.Models;

public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string AccountName { get; set; }
    public int GlobalLimit { get; set; }
}