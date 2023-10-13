using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document
{
    public class DailyCashFlow: IUserResource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public DateOnly Date { get; set; }

        public decimal Amount { get; set; }

        public DailyCashFlow(string userId, DateOnly date, decimal amount)
        {
            UserId = userId;
            Date = date;
            Amount = amount;
        }
    }
}