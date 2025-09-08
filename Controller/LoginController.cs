using Microsoft.AspNetCore.Mvc;
using Task_System.Config;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;
    private readonly JwtGenerator _jwtGenerator;
    private readonly IUserService _userService;

    [HttpPost]
    public async Task<ActionResult<Response>> Login([FromBody] LoginRequest lr)
    {
        l.log($"Received login request for {lr.email} with pw {lr.password}");
        
        User user = await _loginService.LoginAsync(lr);
        l.log($"User {user} logged in successfully");

        string AccessToken = _jwtGenerator.GenerateAccessToken(user.Id);
        RefreshToken refreshToken = _jwtGenerator.GenerateRefreshToken();
        l.log($"Generated access token for user {user.Id}: {AccessToken}");
        l.log($"Refresh token: {refreshToken.Token}, {refreshToken.Expires}");

        user.RefreshToken = refreshToken.Token;
        await _userService.UpdateUserAsync(user);
        Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,      // Not accessible via JavaScript
            Secure = true,        // Only sent over HTTPS
            SameSite = SameSiteMode.Strict, // Prevent CSRF
            Expires = refreshToken.Expires  // Set cookie expiration
        });

        return Ok(new LoginResponse(ResponseType.LOGIN_OK, AccessToken));
    }

    public LoginController(ILogger<LoginService> l, ILoginService loginService, JwtGenerator jwtGenerator, IUserService userService)
    {
        this.l = l;
        _loginService = loginService;
        _jwtGenerator = jwtGenerator;
        _userService = userService;
    }
}
