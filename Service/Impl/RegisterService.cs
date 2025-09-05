using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Service;

namespace Task_System.Service.Impl
{
    public class RegisterService : IRegisterService
    {
        private readonly PostgresqlDbContext _db;
        private readonly IUserService _us;

        public RegisterService(PostgresqlDbContext db, IUserService us)
        {
            _db = db;
            _us = us;
        }

        public async Task Register(User user)
        {
            if (!await ValidateNewUserAsync(user)) {
                throw new Exception("User validation failed");
            }
                _db.Users.Add(user);
                _db.SaveChanges();
                Console.WriteLine("User registered successfully: " + user);
        }

        private async Task<bool> ValidateNewUserAsync(User user)
        {
            var existingUserByName = await _us.GetByNameAsync(user.Name);
            var existingUserByEmail = await _us.GetByEmailAsync(user.Email);
            if (existingUserByName != null)
            {
                throw new Exception("Username already exists");
            }
            if (existingUserByEmail != null)
            {
                throw new Exception("Email already exists");
            }
            return true;
        }
    }
}
