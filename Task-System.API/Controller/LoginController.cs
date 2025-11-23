using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Task_System.Log;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;
using Microsoft.AspNetCore.Http;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly string ExpiryMinutes = Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRY_MINUTES") ?? "2";

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "LOGIN_OK", typeof(LoginResponse))]
    public async Task<ActionResult<Response>> Login([FromBody] LoginRequest lr)
    {
        l.LogDebug($"Received login request {lr}");
        User user = await _loginService.LoginAsync(lr);
        if (user == null)
        {
            l.LogError($"Failed to login user {lr.email}");
            return Unauthorized(new Response(ResponseType.ERROR, $"Failed to login {lr.email}"));
        }
        l.LogDebug($"User {user} logged in successfully");

        var accessToken = _authService.GetAccessTokenByUserId(user.Id);
        RefreshToken refreshToken = await _authService.GenerateRefreshToken(user.Id);

        return Ok(new TokenResponseDto(accessToken, refreshToken));
    }

    public LoginController(ILogger<LoginService> l, ILoginService loginService, IUserService userService, IAuthService authService)
    {
        this.l = l;
        _loginService = loginService;
        _userService = userService;
        _authService = authService;
    }
}
