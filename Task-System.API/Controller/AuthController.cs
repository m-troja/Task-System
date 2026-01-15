using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Task_System.Exception.Tokens;
using Task_System.Exception.UserException;
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
        l.LogDebug($"Received request to regenerate tokens with refresh token {req.RefreshToken}");

        Boolean validated = false;
        try
        {
            validated = await _authService.ValidateRefreshTokenRequest(req);
        }
        catch (InvalidRefreshTokenException ex)
        {
            l.LogDebug($"Validation failed");
            return Unauthorized(new Response(ResponseType.ERROR, "Validation failed"));
        }
        catch (UserNotFoundException ex)
        {
            l.LogDebug($"User not found");
            return NotFound(new Response(ResponseType.ERROR, "User not found"));
        }
        catch (TokenRevokedException ex)
        {
            l.LogDebug($"Refresh token is revoked");
            return Unauthorized(new Response(ResponseType.ERROR, "Refresh token is revoked"));
        }
        catch (TokenExpiredException ex)
        {
            l.LogDebug($"Refresh token expired");
            return Unauthorized(new Response(ResponseType.ERROR, "Refresh token expired"));
        }
        if (validated)
        {
            var accessToken = await _authService.GetAccessTokenByRefreshToken(req.RefreshToken);
            var userByRefreshToken = await _userService.GetUserByRefreshTokenAsync(req.RefreshToken);
            var refreshToken = await _authService.GenerateRefreshToken(userByRefreshToken.Id);

            var saved = await _userService.SaveRefreshTokenAsync(refreshToken);
            var tokenDto = new TokenResponseDto(accessToken, refreshTokenCnv.EntityToDto(refreshToken));
            l.LogDebug($"Tokens for userId {userByRefreshToken.Id} regenerated successfully: {tokenDto}");

            return Ok(tokenDto);
        }
        else
        {
            return Unauthorized(new Response(ResponseType.ERROR, "Validation failed"));
        }
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
