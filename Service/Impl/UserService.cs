using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Service;

namespace Task_System.Service.Impl
{
    public class UserService : IUserService
    {

        private readonly PostgresqlDbContext _db;
        public UserService(PostgresqlDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetByNameAsync(string name)
        {
            return await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task<User> CreateUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            Console.WriteLine("User created successfully: " + user);

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
