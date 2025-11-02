using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_System.Log;
using Task_System.Model.DTO;
using Task_System.Model.Request;
using Task_System.Service;

namespace Task_System.Controller;

[AllowAnonymous]
[ApiController]
[Route("api/v1/chatgpt")]
public class ChatGptController: ControllerBase
{
    private readonly ILogger<ChatGptController> l;
    private readonly IUserService _userService;
    private readonly IIssueService _iss;

    [AllowAnonymous]
    [HttpGet("user/slack-user-id/{slackUserID}")]
    public async Task<UserDto> GetUserBySlackUserId(String slackUserId)
    {
        l.LogInformation($"Received get user by Slack user ID request: {slackUserId}");
        var userDto = await _userService.GetUserBySlackUserIdAsync(slackUserId);
        return userDto;
    }

    [AllowAnonymous]
    [HttpPost("issue/create")]
    public async Task<IssueDto> CreateIssueBySlack([FromBody] SlackCreateIssueRequest scis)
    {
        l.LogInformation("Received create issue by Slack request");
        var issueDto = await _iss.CreateIssueBySlackAsync(scis);
        return issueDto;
    }
    public ChatGptController(IUserService userService, ILogger<ChatGptController> logger, IIssueService iss)
    {
        _userService = userService;
        l = logger;
        _iss = iss;
    }
}
