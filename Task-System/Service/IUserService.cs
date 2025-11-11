using Task_System.Model.DTO;
using Task_System.Model.Entity;

namespace Task_System.Service
{
    public interface IUserService
    {
        Task<int> GetIdBySlackUserId(string slackUserId);
        Task<User> GetByIdAsync(int id);
        Task<User?> TryGetByEmailAsync(string email);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task<UserDto> GetUserBySlackUserIdAsync(string slackUserId);
        Task deleteAllUsers();
        Task deleteUserById(int id);

    }
}
