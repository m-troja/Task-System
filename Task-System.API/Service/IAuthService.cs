using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface IAuthService
{
    AccessToken GetAccessTokenByUserId(int userId);
    Task<RefreshToken> GenerateRefreshToken(int UserId);
    Task<Boolean> ValidateRefreshTokenRequest(RefreshTokenRequest req);
    Task<AccessToken> GetAccessTokenByRefreshToken(string refreshToken);
}
