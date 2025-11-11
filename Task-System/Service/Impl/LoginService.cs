using System.ComponentModel.DataAnnotations;
using Task_System.Exception.LoginException;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Log;
using Task_System.Security;
using System.Threading.Tasks;
using Task_System.Exception.UserException;

namespace Task_System.Service.Impl;

public class LoginService : ILoginService
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginService> l;
    private readonly PasswordService _passwordService;

    public async Task<User> LoginAsync(LoginRequest lr)
    {
        l.LogDebug($"Attempting login for {lr.email.ToLower()} with pw {lr.password}");

        User user = await ValidateCredentials(lr);

        if (user == null)
        {
            l.LogDebug("Returning null from LoginAsync");
            return null;
        }
        return user;

    }

    private async Task<User> ValidateCredentials(LoginRequest lr)
    {

        string email = lr.email.ToLower();
        if (!new EmailAddressAttribute().IsValid(email))
        {
            l.LogDebug("Login failed: invalid email format");
            throw new InvalidEmailOrPasswordException("invalid email format");
        }
        if (string.IsNullOrWhiteSpace(lr.password) || string.IsNullOrWhiteSpace(email))
        {
            l.LogDebug("Login failed: empty email or password");
            throw new InvalidEmailOrPasswordException("empty email or password");
        }
        if (lr.password.Length > 250 || email.Length > 250)
        {
            l.LogDebug("Login failed: email or password too long");
            throw new InvalidEmailOrPasswordException("email or password too long - max 250 chars");
        }

        User user = await _userService.TryGetByEmailAsync(email);
        if (user == null)
        {
            l.LogDebug("Returning null from ValidateCredentials as result of TryGetByEmailAsync");
            return null;
        }
        byte[] UsersSalt = user.Salt;
        string HashedPw = _passwordService.HashPassword(lr.password, UsersSalt);
        l.LogDebug($"Hashed pw: {HashedPw}");
        l.LogDebug($"User's salt: {user.Salt}");
        l.LogDebug($"Found user {user.Email}");

        if (user.Password == HashedPw)
        {
            l.LogDebug($"Login successful for {user.Email}");
            return user;
        }
        else
        {
            if (await IsUserDisabledAsync(lr.email)) { throw new UserDisabledException("User account is disabled"); }

            l.LogDebug("Login failed: wrong password");
            throw new InvalidEmailOrPasswordException("Wrong email or password");
        }

    }

    private async Task<bool> IsUserDisabledAsync(string email)
    {
        User user = await _userService.GetByEmailAsync(email);
        if (user == null) return true;
        if (user.Disabled)
        {
            l.LogDebug($"User {email} is disabled");
            return true;
        }
        return false;
    }
    public LoginService(IUserService userService, ILogger<LoginService> l, PasswordService passwordService, JwtGenerator jwtGenerator)
    {
        _userService = userService;
        this.l = l;
        _passwordService = passwordService;
    }
}
