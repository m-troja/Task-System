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

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<LoginService> l;
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    [HttpPost("regenerate-access-token")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "NEW_ACCESS_TOKEN_ISSUED", typeof(Response))]
    public async Task<ActionResult<Response>> SendAccessTokenByRefreshToken([FromBody] RefreshAccessTokenRequest req)
    {
        string AccessToken = _authService.GetAccessTokenByUserId(req.userId);

  

        return Ok(new Response(ResponseType.NEW_ACCESS_TOKEN_ISSUED, AccessToken));
    }

    public AuthController(ILogger<LoginService> l, ILoginService loginService, IUserService userService, IAuthService authService)
    {
        this.l = l;
        _loginService = loginService;
        _userService = userService;
        _authService = authService;
    }
}
