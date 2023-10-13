using MongoDB.Bson;

namespace Domain.Document
{
    public interface IUserResource : IDocument
    {
        public string UserId { get; }
    }
}