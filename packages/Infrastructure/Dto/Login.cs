using System.Text.Json.Serialization;

namespace Infrastructure.Dto
{
    public class Login
    {
        public string Email { get; }

        public string Password { get; }

        [JsonIgnore]
        public string? Token { get; set; }

        public Login(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}