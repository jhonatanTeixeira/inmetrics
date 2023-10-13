using Domain.Document;
using Domain.Repository;
using MongoDB.Bson;

namespace Application.Service
{
    public class UserService : BaseCrudService<User>, IUserService
    {
        public UserService(IRepository<User> repository) : base(repository)
        {
        }

        public new async Task<User> Save(User user)
        {
            User? currentUser = null;

            if (user.Id != null)
                currentUser = await Repository.FindById(user.Id);

            if (currentUser != null && !VerifyPassword(currentUser.Password, user.Password) || user.Password != null) {
                user.Password = HashPassword(user.Password);
            } else if (currentUser != null && user.Password == null) {
                user.Password = currentUser.Password;
            } else if (user.Password == null) {
                throw new InvalidDataException("User needs a password");
            }

            return await base.Save(user);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashed, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashed);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return (await Repository.Find(d => d.Email == email, "Id", limit: 1)).FirstOrDefault();
        }
    }
}