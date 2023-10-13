using Application.Service;
using Infrastructure.Dto;

namespace Infrastructure.Service
{
    public class Authentication : IAuthentication
    {
        private readonly string JwtSecretKey;

        private readonly IUserService UserService;

        public Authentication(string jwtSecretKey, IUserService userService)
        {
            JwtSecretKey = jwtSecretKey;
            UserService = userService;
        }

        public async Task<bool> Login(Login login)
        {
            var user = await UserService.GetByEmail(login.Email);

            if (user == null || !UserService.VerifyPassword(user.Password, login.Password)) {
                return false;
            }

            login.Token = JwtHelper.GenerateToken(JwtSecretKey, user.Id);

            return true;
        }
    }
}