using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Exception.ProjectException;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Security;
using Task_System.Service;

namespace Task_System.Service.Impl
{
    public class RegisterService : IRegisterService
    {
        private readonly PostgresqlDbContext _db;
        private readonly IUserService _us;
        private readonly IRoleService _rs;
        private readonly PasswordService _passwordService;


        public async Task Register(User user)
        {
            if (!await ValidateNewUserAsync(user)) {
                return ;
            }
            user.Salt = _passwordService.GenerateSalt();
            user.Password = _passwordService.HashPassword(user.Password, user.Salt);
            user.Roles.Add(await _rs.GetRoleByName(Role.ROLE_USER));
            _db.Users.Add(user);
            _db.SaveChanges();
            Console.WriteLine("User registered successfully: " + user);
        }

        private async Task<bool> ValidateNewUserAsync(User user)
        {
            try
            {
                var existingUserByEmail = await _us.GetByEmailAsync(user.Email);
                throw new UserAlreadyExistsException($"Email '{user.Email}' is already registered.");
            }
            catch (UserNotFoundException)
            {
            }

            return true;
        }
        public RegisterService(PostgresqlDbContext db, IUserService us, IRoleService rs, PasswordService passwordService)
        {
            _db = db;
            _us = us;
            _rs = rs;
            _passwordService = passwordService;
        }
    }
}
