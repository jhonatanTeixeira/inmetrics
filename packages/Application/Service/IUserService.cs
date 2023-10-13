using Domain.Document;

namespace Application.Service
{
    public interface IUserService : ICrudService<User>
    {
        public Task<User?> GetByEmail(string email);

        public string HashPassword(string password);

        public bool VerifyPassword(string hashed, string password);
    }
}