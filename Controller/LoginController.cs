using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Task_System.Config;
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

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "LOGIN_OK", typeof(LoginResponse))]
    public async Task<ActionResult<Response>> Login([FromBody] LoginRequest lr)
    {
        l.log($"Received login request for {lr.email} with pw {lr.password}");
        User user = await _loginService.LoginAsync(lr);
        l.log($"User {user} logged in successfully");

        string AccessToken = _authService.GetAccessTokenByUserId(user.Id);
        RefreshToken refreshToken = await _authService.GenerateRefreshToken(user.Id);

        Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,      // Not accessible via JavaScript
            Secure = true,        // Only sent over HTTPS
            SameSite = SameSiteMode.Strict, // Prevent CSRF
            Expires = refreshToken.Expires  // Set cookie expiration
        });

        return Ok(new LoginResponse(ResponseType.LOGIN_OK, AccessToken));
    }

    public LoginController(ILogger<LoginService> l, ILoginService loginService, IUserService userService, IAuthService authService)
    {
        this.l = l;
        _loginService = loginService;
        _userService = userService;
        _authService = authService;
    }
}
