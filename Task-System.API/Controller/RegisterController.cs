using Microsoft.AspNetCore.Mvc;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Task_System.Service.Impl;
using Task_System.Log;
namespace Task_System.Controller;

[ApiController]
[Route("api/v1/register")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _rs;
    private readonly ILogger<RegisterController> l;

    [HttpPost]
    public async Task<ActionResult<Response>> RegisterUser(RegistrationRequest rr)
    {
        l.LogDebug($"Received registration request: {rr}");
        await _rs.Register(rr);
        return await Task.FromResult(new Response(ResponseType.REGISTRATION_OK, rr.Email) );
    }

    public RegisterController(IRegisterService rs, ILogger<RegisterController> l)
    {
        _rs = rs;
        this.l = l;
    }
}
