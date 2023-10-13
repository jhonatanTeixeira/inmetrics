using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Document
{
    public class User : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
        
        public User(string name, string password, string email) 
        {
            Name = name;
            Password = password;
            Email = email;
        }
    }
}