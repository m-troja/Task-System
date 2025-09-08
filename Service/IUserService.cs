using Task_System.Model.Entity;

namespace Task_System.Service
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
    }
}
