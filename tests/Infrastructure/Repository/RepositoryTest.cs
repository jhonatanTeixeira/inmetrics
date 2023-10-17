using System.Linq.Expressions;
using Domain.Document;
using Infrastructure.Repository;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace tests.Infrastructure.Repository
{
    public class RepositoryTest
    {
        [Fact(Skip = "Need to find a way to mock the extension methods the mongo library nedd to use like ToListAsync")]
        public async Task Find_ReturnsExpectedResult()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<User>>();
            var repository = new BaseRepository<User>(mockCollection.Object);
            Expression<Func<User, bool>> filter = u => u.Name == "John";
            var expectedResult = new List<User> { new("Jhon", "123", "john@john.com" ) { Id = "asdasdw2434" } };

            mockCollection.Setup(
                c => c.Find(filter, null).Skip(0).Limit(30).Sort(Builders<User>.Sort.Ascending("Name")).ToListAsync(It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedResult);

            var result = await repository.Find(filter, "Name", 1, 30);

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact(Skip = "Need to find a way to mock the extension methods the mongo library nedd to use like FirstOrDefaultAsync")]
        public async Task FindById_ReturnsExpectedResult()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<User>>();
            var repository = new BaseRepository<User>(mockCollection.Object);
            var userId = "12345ertefgt";
            var expectedResult = new User ("John", "123", "john@john.com") { Id = userId };

            mockCollection.Setup(c => c.Find(u => u.Id == userId, null).Limit(1).FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var result = await repository.FindById(userId);

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }
    }
}