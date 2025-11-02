using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Exception.LoginException;
using Task_System.Exception.ProjectException;
using Task_System.Exception.Registration;
using Task_System.Exception.UserException;
using Task_System.Log;
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
        private readonly ILogger<RegisterService> l;
        private readonly IChatGptService _chatGptService;

        public async Task Register(RegistrationRequest rr)
        {
            if (rr.Email == null || rr.Password == null || rr.FirstName == null || rr.LastName == null)
            {
                l.LogError("Registration failed: Missing required fields");
                throw new System.Exception("Registration failed: Missing required fields");
            }
            l.LogInformation("Received registration request: " + rr);
            
            string email = rr.Email.ToLower() ;

            if (!new EmailAddressAttribute().IsValid(rr.Email))
                throw new RegisterEmailException("Invalid email address");

            byte[] salt = _passwordService.GenerateSalt();
            string password = _passwordService.HashPassword(rr.Password, salt);
            Role role= await _rs.GetRoleByName(Role.ROLE_USER);
            var user = new User(rr.FirstName, rr.LastName, email, password, salt, role);
            if (!await ValidateNewUserAsync(email)) {
                return ;
            }
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> ValidateNewUserAsync(string email)
        {
            var existingUserByEmail = await _us.TryGetByEmailAsync(email);
            if(existingUserByEmail != null)
            {
                l.LogError("Registration failed: Email already registered");
                throw new UserAlreadyExistsException("Registration failed: Email already registered");
            }

            return true;
        }

        public RegisterService(PostgresqlDbContext db, IUserService us, IRoleService rs, PasswordService passwordService, ILogger<RegisterService> logger, IChatGptService chatGptService)
        {
            _db = db;
            _us = us;
            _rs = rs;
            _passwordService = passwordService;
            l = logger;
            _chatGptService = chatGptService;
        }
    }
}
