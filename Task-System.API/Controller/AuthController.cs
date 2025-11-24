using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Task_System.Exception.Tokens;
using Task_System.Log;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly RefreshTokenCnv refreshTokenCnv;

    [HttpPost("regenerate-tokens")]
    public async Task<ActionResult<TokenResponseDto>> RegenerateTokens([FromBody] RefreshTokenRequest req)
    {
        l.LogDebug($"Regenerating tokens for userId {req.UserId} with refresh token {req.RefreshToken}");

        var userById = await _userService.GetByIdAsync(req.UserId);
        if (userById == null)
        {
            l.LogDebug($"User with id {req.UserId} not found");
            
            return NotFound(new Response(ResponseType.ERROR, "User not found"));
        }
        User? userByRefreshToken;
        try
        {
            userByRefreshToken = await _userService.GetUserByRefreshTokenAsync(req.RefreshToken);
        }
        catch ( InvalidRefreshTokenException ex)
        {
            l.LogDebug($"Invalid refresh token {req.RefreshToken} provided for userId {req.UserId}: {ex.Message}");
            
            return Unauthorized(new Response(ResponseType.ERROR, "Invalid refresh token"));
        }
        if (userById != userByRefreshToken)
        {
            l.LogDebug($"Invalid refresh token for userId {req.UserId}");
            
            return Unauthorized(new Response(ResponseType.ERROR, "Invalid refresh token"));
        }

        var accessToken = _authService.GetAccessTokenByUserId(req.UserId);
        var refreshToken = await _authService.GenerateRefreshToken(req.UserId);

        l.LogDebug($"Generated new access token {accessToken} and refresh token {refreshToken.Token} for userId {req.UserId}");

        return Ok(new TokenResponseDto(accessToken, refreshTokenCnv.EntityToDto(refreshToken)));
    }

    public AuthController(ILogger<LoginService> l, ILoginService loginService, IUserService userService, IAuthService authService, RefreshTokenCnv refreshTokenCnv)
    {
        this.l = l;
        _loginService = loginService;
        _userService = userService;
        _authService = authService;
        this.refreshTokenCnv = refreshTokenCnv;
    }
}
