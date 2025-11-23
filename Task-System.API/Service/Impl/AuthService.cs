using Task_System.Log;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
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

    public async Task<RefreshToken> GenerateRefreshToken(int UserId)
    {
        User userByUserId = await _userService.GetByIdAsync(UserId);
        
        var NewRefreshToken = _jwtGenerator.GenerateRefreshToken(UserId);
        l.LogDebug($"Generated refresh token for userId {userByUserId.Id}: {NewRefreshToken.Token}, expires: {NewRefreshToken.Expires}");
        userByUserId.RefreshToken = NewRefreshToken.Token;
        await _userService.UpdateUserAsync(userByUserId);

        return NewRefreshToken;
    }

    public AuthService(PostgresqlDbContext db, ILogger<AuthService> l, IJwtGenerator jwtGenerator, IUserService userService)
    {
        _db = db;
        this.l = l;
        _jwtGenerator = jwtGenerator;
        _userService = userService;
    }
}
