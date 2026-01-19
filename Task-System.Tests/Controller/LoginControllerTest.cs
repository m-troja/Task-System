using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Task_System.Controller;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Controller
{
    public class LoginControllerTest
    {
        private LoginController CreateController(
            Mock<ILoginService> loginService,
            Mock<IUserService> userService,
            Mock<IAuthService> authService)
        {   
            var refreshTokenCnv = new RefreshTokenCnv();
            var logger = new LoggerFactory().CreateLogger<LoginService>();
            return new LoginController(logger, loginService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnTokenResponse_WhenCredentialsAreValid()
        {
            // arrange
            var loginService = new Mock<ILoginService>();
            var userService = new Mock<IUserService>();
            var authService = new Mock<IAuthService>();

            var controller = CreateController(loginService, userService, authService);

            var request = new LoginRequest("user@test.com", "password");
            var user = new User { Id = 5, Email = "user@test.com" };
            var accessToken = new AccessToken("new-access-token", DateTime.UtcNow.AddMinutes(2));
            TokenResponseDto tokenResponseDto = new TokenResponseDto( accessToken, 
                new Model.DTO.RefreshTokenDto("token", DateTime.UtcNow.AddDays(7))
            );
            loginService.Setup(s => s.LoginAsync(request)).ReturnsAsync(tokenResponseDto);
            authService.Setup(a => a.GetAccessTokenByUserId(5)).Returns(accessToken);
            authService.Setup(a => a.GenerateRefreshToken(5))
                       .ReturnsAsync(new RefreshToken("new-refresh-token", 5, DateTime.Parse("2025-01-01")));

            // act
            var result = await controller.Login(request);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TokenResponseDto>(ok.Value);

            Assert.Equal("new-access-token", dto.AccessToken.Token);
            Assert.Equal("new-refresh-token", dto.RefreshToken.Token);

            loginService.Verify(s => s.LoginAsync(request), Times.Once);
            authService.Verify(a => a.GetAccessTokenByUserId(5), Times.Once);
            authService.Verify(a => a.GenerateRefreshToken(5), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserIsNull()
        {
            // arrange
            var loginService = new Mock<ILoginService>();
            var userService = new Mock<IUserService>();
            var authService = new Mock<IAuthService>();

            var controller = CreateController(loginService, userService, authService);

            var request = new LoginRequest("user@test.com", "pw123");

            var accessToken = new AccessToken("new-access-token", DateTime.UtcNow.AddMinutes(2));
            TokenResponseDto tokenResponseDto = new TokenResponseDto(accessToken,
                new Model.DTO.RefreshTokenDto("token", DateTime.UtcNow.AddDays(7))
            );
            loginService.Setup(s => s.LoginAsync(request)).ReturnsAsync(tokenResponseDto);

            // act
            var result = await controller.Login(request);

            // assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<Response>(unauthorized.Value);

            Assert.Equal(ResponseType.ERROR, response.responseType);
            Assert.Contains("user@test.com", response.message);

            authService.Verify(a => a.GetAccessTokenByUserId(It.IsAny<int>()), Times.Never);
            authService.Verify(a => a.GenerateRefreshToken(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Login_ShouldReturnNullAccessToken_WhenAuthServiceFails()
        {
            // arrange
            var loginService = new Mock<ILoginService>();
            var userService = new Mock<IUserService>();
            var authService = new Mock<IAuthService>();

            var controller = CreateController(loginService, userService, authService);

            var request = new LoginRequest("user@test.com", "pw");
            var user = new User { Id = 77, Email = "user@test.com" };

            var accessToken = new AccessToken("new-access-token", DateTime.UtcNow.AddMinutes(2));
            TokenResponseDto tokenResponseDto = new TokenResponseDto(accessToken,
                new Model.DTO.RefreshTokenDto("token", DateTime.UtcNow.AddDays(7))
            );
            loginService.Setup(s => s.LoginAsync(request)).ReturnsAsync(tokenResponseDto);
            authService.Setup(a => a.GetAccessTokenByUserId(77)).Returns<string>(null);
            authService.Setup(a => a.GenerateRefreshToken(77))
                       .ReturnsAsync(new RefreshToken("TOK", 77, DateTime.UtcNow));

            // act
            var result = await controller.Login(request);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TokenResponseDto>(ok.Value);

            Assert.Null(dto.AccessToken);
            Assert.Equal("TOK", dto.RefreshToken.Token);

            loginService.Verify(s => s.LoginAsync(request), Times.Once);
            authService.Verify(a => a.GetAccessTokenByUserId(77), Times.Once);
            authService.Verify(a => a.GenerateRefreshToken(77), Times.Once);
        }
    }
}
