using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Task_System.Log;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;
using Microsoft.AspNetCore.Http;
using Task_System.Model.DTO.Cnv;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;

    [HttpPost]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "LOGIN_OK", typeof(LoginResponse))]
    public async Task<ActionResult<Response>> Login([FromBody] LoginRequest lr)
    {
        l.LogDebug($"Received login request {lr}");

        var tokenDto = await _loginService.LoginAsync(lr);
        if (tokenDto == null)
        {
            l.LogError($"Failed to login user {lr.email}");
            return Unauthorized(new Response(ResponseType.ERROR, $"Failed to login {lr.email}"));
        }
        l.LogDebug($"User {lr.email} logged in successfully");

        return Ok(tokenDto);
    }

    public LoginController(ILogger<LoginService> l, ILoginService loginService)
    {
        this.l = l;
        _loginService = loginService;
    }
}
