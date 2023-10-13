
using MongoDB.Bson;

namespace Domain.Document
{
    public interface IDocument
    {
        public string? Id { get; set ;}
    }
}