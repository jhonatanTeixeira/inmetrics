using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document
{
    public class Transaction : IUserResource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public bool Emitted { get; set; } = false;

        public Transaction(string userId, decimal amount, DateTime date)
        {
            UserId = userId;
            Amount = amount;
            Date = date;
        }
    }
}