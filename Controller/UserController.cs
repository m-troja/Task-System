using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Task_System.Log;
using Task_System.Model;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _us;
        private readonly UserCnv _userCnv;
        private readonly ILogger<UserController> l;

        public UserController(IUserService us, UserCnv userCnv, ILogger<UserController> logger)
        {
            _us = us;
            _userCnv = userCnv;
            l = logger;
        }

        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            l.log($"Fetching user by id: {id}");
            var user =  await _us.GetByIdAsync(id);

            return Ok(_userCnv.ConvertUserToDto(user));
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            l.log($"Fetching user by email: {email}");
            var user = await _us.GetByEmailAsync(email);

            return Ok(_userCnv.ConvertUserToDto(user)); ;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            l.log("Fetching all users");
            var users = await _us.GetAllUsersAsync();
            return Ok(_userCnv.ConvertUsersToUsersDto(users));
        }
    }
}
