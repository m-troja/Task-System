using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Task_System.Log;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> l;
    private readonly IUserService _us;
    private readonly UserCnv _userCnv;

    [HttpGet("env")]
    public ActionResult<String> TestDeploy()
    {
        //Env vars check
        string env = "";
        foreach (System.Collections.DictionaryEntry envVar in Environment.GetEnvironmentVariables())
        {
            string key = envVar.Key.ToString() ?? "";
            if (key.StartsWith("TS_"))
            {
                env = env + envVar.Key + " = " + envVar.Value + "\n";
                l.log($"EnvVar: {envVar.Key} = {envVar.Value}");
            }

        }
        return Ok(env); 
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfileByAccessToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { message = "Invalid token: missing user ID" });
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Invalid user ID in token" });
        }

        var user = await _us.GetByIdAsync(userId);
        
        return Ok(_userCnv.ConvertUserToDto(user));
    }
    [Authorize]
    [HttpGet("profile-authorized")]
    public async Task<ActionResult<UserDto>> AuthorizedGetProfileByAccessToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { message = "Invalid token: missing user ID" });
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Invalid user ID in token" });
        }

        var user = await _us.GetByIdAsync(userId);

        return Ok(_userCnv.ConvertUserToDto(user));
    }
    public TestController(ILogger<TestController> l, IUserService us, UserCnv userCnv)
    {
        this.l = l;
        _us = us;
        _userCnv = userCnv;
    }
}
