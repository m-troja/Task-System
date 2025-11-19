using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;

namespace Task_System.Service;

public interface IAuthService
{
    string GetAccessTokenByUserId(int userId);
    Task<RefreshToken> GenerateRefreshToken(int UserId);

}
