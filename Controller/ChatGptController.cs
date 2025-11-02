using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_System.Log;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Request;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/chatgpt")]
public class ChatGptController: ControllerBase
{
    private readonly ILogger<ChatGptController> l;
    private readonly IUserService _userService;
    private readonly IIssueService _iss;
    private readonly IssueCnv _issueCnv;

    [HttpGet("user/slack-user-id/{slackUserID}")]
    public async Task<UserDto> GetUserBySlackUserId(String slackUserId)
    {
        l.LogInformation($"Received get user by Slack user ID request: {slackUserId}");
        var userDto = await _userService.GetUserBySlackUserIdAsync(slackUserId);
        return userDto;
    }

    [HttpPost("issue/create")]
    public async Task<IssueDtoChatGpt> CreateIssueBySlack([FromBody] SlackCreateIssueRequest scis)
    {
        l.LogInformation("Received create issue by Slack request");
        var issueDto = await _iss.CreateIssueBySlackAsync(scis);
        return issueDto;
    }
    [HttpPut("issue/assign")]
    public async Task<IssueDtoChatGpt> AssignIssueByChatGpt([FromBody] AssignIssueRequestChatGpt req)
    {
        l.LogInformation($"Received AssignIssueBySlack request: {req}");
        var issue = await _iss.AssignIssueBySlackAsync(req);
        return _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
    }

    public ChatGptController(IUserService userService, ILogger<ChatGptController> logger, IIssueService iss, IssueCnv _issueCnv)
    {
        _userService = userService;
        l = logger;
        _iss = iss;
        this._issueCnv = _issueCnv;
    }
}
