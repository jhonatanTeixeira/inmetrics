using MongoDB.Bson;

namespace Infrastructure.Service
{
    public class UserContext
    {
         private static readonly AsyncLocal<string?> _userId = new();

        public string? UserId
        {
            get => _userId.Value;
            set => _userId.Value = value;
        }
    }
}