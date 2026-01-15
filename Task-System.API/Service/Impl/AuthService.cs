using Microsoft.EntityFrameworkCore;
using Task_System.Data;
using Task_System.Exception.Tokens;
using Task_System.Exception.UserException;
using Task_System.Log;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Security;
namespace Task_System.Service.Impl;

public class AuthService : IAuthService
{
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<AuthService> l;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUserService _userService;

    public AccessToken GetAccessTokenByUserId(int userId)
    {
        var AccessToken =  _jwtGenerator.GenerateAccessToken(userId);
        l.LogDebug($"Generated access token for user {userId}: {AccessToken}");
        return AccessToken;     
    }

    public async Task<AccessToken> GetAccessTokenByRefreshToken(string refreshToken)
    {
        var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
        var AccessToken = _jwtGenerator.GenerateAccessToken(user.Id);
        l.LogDebug($"Generated access token for user {user.Id}: {AccessToken}");
        return AccessToken;
    }


    public async Task<RefreshToken> GenerateRefreshToken(int UserId)
    {
        User userByUserId = await _userService.GetByIdAsync(UserId);
        
        var NewRefreshToken = _jwtGenerator.GenerateRefreshToken(UserId, await _userService.GetByIdAsync(UserId));
        l.LogDebug($"Generated refresh token for userId {userByUserId.Id}: {NewRefreshToken.Token}, expires: {NewRefreshToken.Expires}");
        userByUserId.RefreshTokens.Add(NewRefreshToken);
        await _userService.UpdateUserAsync(userByUserId);

        return NewRefreshToken;
    }

    public async Task<Boolean> ValidateRefreshTokenRequest(RefreshTokenRequest req)
    {
        //var userById = await _db.Users.FirstAsync(u => u.Id == req.UserId);
        //if (userById == null)
        //{
        //    l.LogDebug($"User with id {req.UserId} not found");
        //    throw new UserNotFoundException("User not found");
        //}
        var userByRefreshToken = await _db.Users.FirstOrDefaultAsync( u => u.RefreshTokens.Any(rt => rt.Token == req.RefreshToken));
        var refreshToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == req.RefreshToken);
        //if (userByRefreshToken == null || refreshToken == null || refreshToken.UserId != userByRefreshToken.Id || refreshToken.UserId != req.UserId)
        if (userByRefreshToken == null || refreshToken == null || refreshToken.UserId != userByRefreshToken.Id )
            {
            l.LogDebug("Refresh token or user not found");
            throw new InvalidRefreshTokenException("Refresh token or user not found");
        }
        if (refreshToken.IsRevoked)
        {
            l.LogDebug("Refresh token is revoked");
            throw new TokenRevokedException("Refresh token is revoked");
        }
        if (refreshToken.Expires < DateTime.UtcNow)
        {
            l.LogDebug("Refresh token expired");
            throw new TokenExpiredException("Refresh token expired");
        }
        l.LogDebug("Token validated succesffully");
        return true;
    }


    public AuthService(PostgresqlDbContext db, ILogger<AuthService> l, IJwtGenerator jwtGenerator, IUserService userService)
    {
        _db = db;
        this.l = l;
        _jwtGenerator = jwtGenerator;
        _userService = userService;
    }
}
