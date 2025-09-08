using System.ComponentModel.DataAnnotations;
using Task_System.Exception.LoginException;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Config;

namespace Task_System.Service.Impl;

public class LoginService : ILoginService
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginService> l;
    public async Task<User> LoginAsync(LoginRequest lr)
    {
        l.log($"Attempting login for {lr.email} with pw {lr.password}");

        if (!new EmailAddressAttribute().IsValid(lr.email))
            throw new InvalidEmailOrPasswordException("Wrong email or password");

        User user = await _userService.GetByEmailAsync(lr.email);
        l.log($"Found user {user}");
        if (user.Password == lr.password)
        {
            l.log($"Login successful for {user}");
            return user;
        }
        else
        {
            l.log("Login failed: wrong password");
            throw new InvalidEmailOrPasswordException("Wrong email or password");
        }
    }

    public LoginService(IUserService userService, ILogger<LoginService> l)
    {
        _userService = userService;
        this.l = l;
    }
}
