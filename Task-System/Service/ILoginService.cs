using Task_System.Model.Entity;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface ILoginService
{
    Task<User> LoginAsync(LoginRequest lr);
}
