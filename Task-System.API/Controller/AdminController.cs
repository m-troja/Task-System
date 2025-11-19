using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("/v1/api/admin/")]
public class AdminController : ControllerBase
{
    private readonly IIssueService _issueService;
    private readonly IUserService _userService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IIssueService issueService, ILogger<AdminController> logger, IUserService userService)
    {
        _issueService = issueService;
        _logger = logger;
        _userService = userService;
    }

    [HttpDelete("issue/all")]
    public async Task<ActionResult<string>> DeleteAllIssues()
    {
        _logger.LogInformation("Triggered endpoint Delete all issues");
        await _issueService.deleteAllIssues();
        return Ok("All issues deleted successfully");
    }

    [HttpDelete("issue/{id:int}")]
    public async Task<ActionResult<string>> DeletelIssueById(int id)
    {
        _logger.LogInformation($"Triggered endpoint DeletelIssueById {id}");
        await _issueService.deleteIssueById(id);
        return Ok($"Deleted issue {id}");
    }
    
    [HttpDelete("user/all")]
    public async Task<ActionResult<string>> DeleteAllUsers()
    {
        _logger.LogInformation("Triggered endpoint Delete all users");
        await _userService.deleteAllUsers();
        return Ok("All users deleted successfully");
    }
    
    [HttpDelete("user/{id:int}")]
    public async Task<ActionResult<string>> DeletelUserById(int id)
    {
        _logger.LogInformation($"Triggered endpoint DeletelUserById {id}");
        await _userService.deleteUserById(id);
        return Ok($"Deleted user {id}");
    }
}