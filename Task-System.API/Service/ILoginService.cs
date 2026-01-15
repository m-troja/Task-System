using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;

namespace Task_System.Service;

public interface ILoginService
{
    Task<TokenResponseDto> LoginAsync(LoginRequest lr);
}
