using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Response;

namespace Task_System.Service;

public interface IAuthService
{
    AccessToken GetAccessTokenByUserId(int userId);
    Task<RefreshToken> GenerateRefreshToken(int UserId);
    Task<Boolean> ValidateRefreshTokenRequest(string refreshToken);
    Task<AccessToken> GetAccessTokenByRefreshToken(string refreshToken);
    Task<TokenResponseDto> RegenerateTokensByRefreshToken(string refreshToken);
}
