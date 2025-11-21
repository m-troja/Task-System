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
    private readonly ILogger<LoginService> logger;
    private readonly PasswordService _passwordService;

    public LoginService(IUserService userService, ILogger<LoginService> logger, PasswordService passwordService, IJwtGenerator jwtGenerator)
    {
        _userService = userService;
        this.logger = logger;
        _passwordService = passwordService;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        logger.LogDebug($"Attempting login for {request.email.ToLower()}");

        ValidateInput(request);

        var user = await _userService.TryGetByEmailAsync(request.email.ToLower());
        if (user == null)
        {
            logger.LogDebug("Login failed: user not found");
            throw new InvalidEmailOrPasswordException("Wrong email or password");
        }

        if (user.Disabled)
        {
            logger.LogDebug($"Login failed: user {user.Email} is disabled");
            throw new UserDisabledException("User account is disabled");
        }

        if (user.Salt == null)
            throw new ArgumentNullException(nameof(user.Salt));

        var hashedPassword = _passwordService.HashPassword(request.password, user.Salt);

        if (hashedPassword != user.Password)
        {
            logger.LogDebug("Login failed: wrong password");
            throw new InvalidEmailOrPasswordException("Wrong email or password");
        }

        logger.LogDebug($"Login successful for {user.Email}");
        return user;
    }

    private void ValidateInput(LoginRequest request)
    {
        if (!new EmailAddressAttribute().IsValid(request.email))
        {
            logger.LogDebug("Login failed: invalid email format");
            throw new InvalidEmailOrPasswordException("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(request.password) || string.IsNullOrWhiteSpace(request.email))
        {
            logger.LogDebug("Login failed: empty email or password");
            throw new InvalidEmailOrPasswordException("Email or password cannot be empty");
        }

        if (request.password.Length > 250 || request.email.Length > 250)
        {
            logger.LogDebug("Login failed: email or password too long");
            throw new InvalidEmailOrPasswordException("Email or password too long - max 250 chars");
        }
    }
}
