using System.Linq.Expressions;
using Application.Service;
using Domain.Document;
using Domain.Repository;
using Infrastructure.Dto;
using Infrastructure.Service;
using Moq;
using Xunit;

namespace tests.Infrastructure.Service
{
    public class AuthenticationTest
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsTrue()
        {
            var repositoryMock = new Mock<IRepository<User>>();
            var userService = new UserService(repositoryMock.Object);
            var user = new User("test",  userService.HashPassword("password"), "test@example.com");
            user.Id = "12434ewddfgb656rh567";

            repositoryMock.Setup(x => x.Find(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<User> { user });


            var authentication = new Authentication("this neeeds to be a very long jwt secret key", userService);
            var validLogin = new Login("test@example.com", "password");

            var result = await authentication.Login(validLogin);

            Assert.True(result);
            Assert.NotNull(validLogin.Token);
        }
    }
}