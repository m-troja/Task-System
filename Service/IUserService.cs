using Task_System.Model.Entity;

namespace Task_System.Service
{
    public interface IUserService
    {
        Task<User> GetByNameAsync(string name);
        Task<User> GetByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> GetByEmailAsync(string email);
    }
}
