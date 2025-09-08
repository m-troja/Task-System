using Microsoft.AspNetCore.Mvc;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller
{
    [ApiController]
    [Route("api/v1/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _rs;

        [HttpPost]
        public async Task<ActionResult<Response>> RegisterUser(RegistrationRequest rr)
        {
            await _rs.Register(rr);
            return await Task.FromResult(new Response(ResponseType.REGISTRATION_OK, rr.Email) );
        }

        public RegisterController(IRegisterService rs)
        {
            _rs = rs;
        }
    }
}
