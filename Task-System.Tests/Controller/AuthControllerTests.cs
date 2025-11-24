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
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Controller;

public class AuthControllerTests
{
    private readonly Mock<ILogger<LoginService>> _mockLogger = new();
    private readonly Mock<ILoginService> _mockLoginService = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IAuthService> _mockAuthService = new();

    private AuthController CreateController() {
        var refreshTokenCnv = new RefreshTokenCnv();
        return new AuthController (_mockLogger.Object, _mockLoginService.Object, _mockUserService.Object, _mockAuthService.Object, refreshTokenCnv);
    }

    [Fact]
    public async Task RegenerateTokens_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // given
        var user = new User("TestUser", "U1") { Id = 1 };
        var refreshToken = new RefreshToken("valid-token", user.Id, DateTime.UtcNow);
        var req = new RefreshTokenRequest(1, refreshToken.Token);
        var accessToken = new AccessToken("new-access-token", DateTime.UtcNow.AddMinutes(2));
        var refreshTokenCnv = new RefreshTokenCnv();
        var tokenResponseDto = new TokenResponseDto(accessToken, refreshTokenCnv.EntityToDto(refreshToken));

        _mockUserService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
        _mockUserService.Setup(x => x.GetUserByRefreshTokenAsync(refreshToken.Token)).ReturnsAsync(user);
        _mockAuthService.Setup(x => x.GetAccessTokenByUserId(1)).Returns(accessToken);
        _mockAuthService.Setup(x => x.GenerateRefreshToken(1)).ReturnsAsync(refreshToken);

        var controller = CreateController();

        // when
        var result = await controller.RegenerateTokens(req);

        // then
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var tokenResponse = Assert.IsType<TokenResponseDto>(okResult.Value);

        Assert.Equal(accessToken, tokenResponse.AccessToken);
        Assert.Equal(refreshToken.Token, tokenResponse.RefreshToken.Token);
    }

    [Fact]
    public async Task RegenerateTokens_ShouldReturnUnauthorized_WhenRefreshTokenIsInvalid()
    {
        // given
        var user = new User("TestUser", "U123");
        var req = new RefreshTokenRequest(1, "invalid-token");

        _mockUserService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);

        var controller = CreateController();

        // when
        var result = await controller.RegenerateTokens(req);

        // then
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<Response>(unauthorizedResult.Value);

        Assert.Equal(ResponseType.ERROR, response.responseType);
        Assert.Equal("Invalid refresh token", response.message);
    }
}