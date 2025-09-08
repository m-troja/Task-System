using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Exception.ProjectException;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Model.Request;
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


        public async Task Register(RegistrationRequest rr)
        {
            string email = rr.Email.ToLower();
            byte[] salt = _passwordService.GenerateSalt();
            string password = _passwordService.HashPassword(rr.Password, salt);
            
            var user = new User(rr.FirstName, rr.LastName, email, password);
            user.Salt = salt;
            user.Password = password;
            user.Roles.Add(await _rs.GetRoleByName(Role.ROLE_USER));

            if (!await ValidateNewUserAsync(user)) {
                return ;
            }

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
