using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Task_System.Exception.LoginException;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service
{
    public class LoginServiceTests
    {
        private readonly Mock<IUserService> _mockUserService = new();
        private readonly Mock<ILogger<LoginService>> _mockLogger = new();
        private readonly PasswordService _passwordService;

        public LoginServiceTests()
        {
            _passwordService = new PasswordService(new LoggerFactory().CreateLogger<PasswordService>());
        }

        private LoginService CreateService()
        {
            return new LoginService(_mockUserService.Object, _mockLogger.Object, _passwordService, null);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "securePassword";
            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(password, salt);

            var user = new User("TestUser", "test@example.com")
            {
                Id = 1,
                Salt = salt,
                Password = hashedPassword,
                Disabled = false
            };

            var request = new LoginRequest("test@example.com", password);

            _mockUserService.Setup(x => x.TryGetByEmailAsync("test@example.com")).ReturnsAsync(user);

            var service = CreateService();

            // Act
            var result = await service.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidEmailOrPasswordException_WhenPasswordIncorrect()
        {
            // Arrange
            var password = "correctPassword";
            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(password, salt);

            var user = new User("TestUser", "test@example.com")
            {
                Id = 1,
                Salt = salt,
                Password = hashedPassword,
                Disabled = false
            };

            var request = new LoginRequest("test@example.com", "wrongPassword");

            _mockUserService.Setup(x => x.TryGetByEmailAsync("test@example.com")).ReturnsAsync(user);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUserDisabledException_WhenUserIsDisabled()
        {
            // Arrange
            var password = "password";
            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(password, salt);

            var user = new User("TestUser", "test@example.com")
            {
                Id = 1,
                Salt = salt,
                Password = hashedPassword,
                Disabled = true
            };

            var request = new LoginRequest("test@example.com", password);

            _mockUserService.Setup(x => x.TryGetByEmailAsync("test@example.com")).ReturnsAsync(user);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<UserDisabledException>(() => service.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidEmailOrPasswordException_WhenEmailInvalid()
        {
            // Arrange
            var request = new LoginRequest("invalid-email", "password");
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidEmailOrPasswordException_WhenEmailOrPasswordEmpty()
        {
            // Arrange
            var service = CreateService();

            var request1 = new LoginRequest("", "password");
            var request2 = new LoginRequest("test@example.com", "");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request1));
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request2));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidEmailOrPasswordException_WhenEmailOrPasswordTooLong()
        {
            // Arrange
            var longEmail = new string('a', 251) + "@example.com";
            var longPassword = new string('p', 251);

            var service = CreateService();

            var request1 = new LoginRequest(longEmail, "password");
            var request2 = new LoginRequest("test@example.com", longPassword);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request1));
            await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() => service.LoginAsync(request2));
        }
    }
}
