using System.ComponentModel.DataAnnotations;
using Task_System.Exception.LoginException;
using Task_System.Model.Entity;
using Task_System.Model.Request;

namespace Task_System.Service.Impl;

public class LoginService : ILoginService
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginService> l;
    public async Task<User> LoginAsync(LoginRequest lr)
    {
        if (!new EmailAddressAttribute().IsValid(lr.email))
            throw new InvalidEmailOrPasswordException("Wrong email or password");

        User user = await _userService.GetByEmailAsync(lr.email);
        if (user.Password == lr.password) return user;
        else throw new InvalidEmailOrPasswordException("Wrong email or password");
    }

    public LoginService(IUserService userService, ILogger<LoginService> l)
    {
        _userService = userService;
        this.l = l;
    }
}
