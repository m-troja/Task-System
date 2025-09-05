using Microsoft.AspNetCore.Mvc;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller
{
    [ApiController]
    [Route("v1/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _rs;

        [HttpPost]
        public async Task<ActionResult<Response>> RegisterUser(RegistrationRequest rr)
        {
            var user = new User(rr.Name, rr.Email, rr.Password);
            await _rs.Register(user);
            return await Task.FromResult(new Response(ResponseType.REGISTRATION_RESULT, "RegisterController is working!") );
        }

        public RegisterController(IRegisterService rs)
        {
            _rs = rs;
        }
    }
}
