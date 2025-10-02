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
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    [HttpPost("regenerate-tokens")]
    public async Task<ActionResult<Response>> RegenerateTokens([FromBody] RefreshAccessTokenRequest req)
    {
        l.log($"Regenerating tokens for userId {req.UserId} with refresh token {req.RefreshToken}");
        User user = await _userService.GetByIdAsync(req.UserId);
        if (user.RefreshToken != req.RefreshToken)
        {
            l.log($"Invalid refresh token for userId {req.UserId}");
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        string accessToken = _authService.GetAccessTokenByUserId(req.UserId);
        RefreshToken refreshToken = await _authService.GenerateRefreshToken(req.UserId);
        l.log($"Generated new access token {accessToken} and refresh token {refreshToken.Token} for userId {req.UserId}");
        return Ok(new { accessToken, refreshToken });
    }

    public AuthController(ILogger<LoginService> l, ILoginService loginService, IUserService userService, IAuthService authService)
    {
        this.l = l;
        _loginService = loginService;
        _userService = userService;
        _authService = authService;
    }
}
