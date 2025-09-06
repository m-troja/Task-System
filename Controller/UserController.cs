using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Task_System.Model;
using Task_System.Model.Entity;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user =  await _userService.GetByIdAsync(id);
            
            Console.WriteLine("Controller: " + user);

            return Ok(user);
        }

        [HttpGet("create")]
        public async Task<ActionResult<User>> CreateUser() {  
            var user = new User("michal", "trojanowski", "michal@email.com", "password");

            var userAsync = await _userService.CreateUserAsync(user);
            

            Console.WriteLine("Controller: " + userAsync.LastName);

            return Ok(userAsync);
        }
    }
}
