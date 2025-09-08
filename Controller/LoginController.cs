using Microsoft.AspNetCore.Mvc;
using Task_System.Config;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest lr)
    {
        l.log($"Received login request for {lr.email} with pw {lr.password}");
        
        User user = await _loginService.LoginAsync(lr);

        l.log($"User {user} logged in successfully");

        return Ok("Login successful");
    }

    public LoginController(ILogger<LoginService> l, ILoginService loginService)
    {
        this.l = l;
        _loginService = loginService;
    }
}
