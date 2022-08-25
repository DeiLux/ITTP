using Moq;
using Xunit;
using ITTP.Core.Models;
using ITTP.Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ITTP.Database.Context;
using ITTP.Database.Repositories;

namespace ITTP.Services.UserService.Tests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService(GetInMemoryUserRepository(),
                new Mock<ILogger<UserService>>().Object);
        }

        private IUserRepository GetInMemoryUserRepository()
        {
            return new UserRepository(new NpgSqlContext(new DbContextOptionsBuilder<NpgSqlContext>()
                .UseInMemoryDatabase("ITTPTests")
                .Options));
        }

        private User CreateUser(string login = "Gena",
            string password = "Gena",
            int birthday = -22)
        {
            return new User()
            {
                Guid = Guid.NewGuid(),
                Login = login,
                Password = password,
                Name = "Gena",
                Gender = 1,
                Birthday = DateTime.UtcNow.AddYears(birthday),
                IsAdmin = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = null,
                ModifiedOn = null,
                ModifiedBy = null,
                RevokedOn = null,
                RevokedBy = null,
            };
        }

        [Fact]
        public async Task CreateUserTestsTrueAsync()
        {
            // Arrange
            var user = CreateUser("CreateUserTrue");

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserTestsFalseAsync()
        {
            // Arrange
            var user = CreateUser("Gena123456");

            // Act
            await _userService.CreateUserAsync(user);
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.True(!result);
        }

        [Fact]
        public async Task DeleteUserFullTestsTrueAsync()
        {
            // Arrange
            var user = CreateUser("");
            await _userService.CreateUserAsync(user);

            // Act
            var result = await _userService.DeleteUserFullAsync(user.Login!);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserFullTestsFalseAsync()
        {
            // Arrange
            var user = CreateUser();
            await _userService.CreateUserAsync(user);

            // Act
            var result = await _userService.DeleteUserFullAsync(user.Login!);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ReadUserTestsAsync()
        {
            // Arrange
            var user = CreateUser("deilux", "deilux");
            await _userService.CreateUserAsync(user);

            // Act
            var result1 = await _userService.ReadUserAsync(user.Login!, user.Password!);
            var result2 = await _userService.ReadUserAsync(user.Guid);
            var result3 = await _userService.ReadUserAsync(user.Login!);

            // Assert
            Assert.True(user.Equals(result1));
            Assert.True(user.Equals(result2));
            Assert.True(user.Equals(result3));
        }

        [Fact]
        public async Task ReadUserTestsNullAsync()
        {
            // Arrange && Act
            var result = await _userService.ReadUserAsync("deilux123", "deilux");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReadUsersTestsNotNullAsync()
        {
            // Arrange
            var user1 = CreateUser("deilux098");
            var user2 = CreateUser("deilux0987");
            await _userService.CreateUserAsync(user1);
            await _userService.CreateUserAsync(user2);

            // Act
            var result = await _userService.ReadUsersAsync();

            // Assert
            Assert.True(result.Any());
        }

        [Fact]
        public async Task ReadUsersAgeTestsNotNullAsync()
        {
            // Arrange
            var user1 = CreateUser("dei1234", "dei", -40);
            var user2 = CreateUser("dei12345", "dei", -40);
            await _userService.CreateUserAsync(user1);
            await _userService.CreateUserAsync(user2);

            // Act
            var users = await _userService.ReadUsersAgeAsync(38);

            // Assert
            Assert.True(users.Any());
        }

        [Fact]
        public async Task ReadUsersRevokedOnNullTestsNotNullAsync()
        {
            // Arrange
            var user1 = CreateUser("deilux09876");
            var user2 = CreateUser("deilux098765");
            await _userService.CreateUserAsync(user1);
            await _userService.CreateUserAsync(user2);

            // Act
            var result = await _userService.ReadUsersRevokedOnNullAsync();

            // Assert
            Assert.True(result.Any());
        }
    }
}
