using Task_System.Model.DTO;
using Task_System.Model.DTO.ChatGpt;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.IssueFolder;
using System.Net.Http.Json;
using Task_System.Tools;
namespace Task_System.Service.Impl;

public class SlackNotificationService : ISlackNotificationService
{
    private readonly IssueCnv _issueCnv;
    private readonly ILogger<SlackNotificationService> logger;
    private readonly string _EventEndpoint = "/api/v1/task-system/event";
    private readonly String _ChatServerAddress = Environment.GetEnvironmentVariable("CHAT_SERVER_ADDRESS") ?? "localhost";
    private readonly String _ChatServerPort = Environment.GetEnvironmentVariable("CHAT_SERVER_PORT") ?? "6969";
    private readonly string _ChatServerUri = "http://" +  Environment.GetEnvironmentVariable("CHAT_SERVER_ADDRESS") + ":" + Environment.GetEnvironmentVariable("CHAT_SERVER_PORT") + "/api/v1/task-system/event";
    private readonly HttpClient _httpClient;
    public SlackNotificationService(ILogger<SlackNotificationService> logger, IssueCnv _issueCnv, HttpClient _httpClient)

    {
        this._httpClient = _httpClient;
        this.logger = logger;
        this._issueCnv = _issueCnv;
    }
    public async Task SendIssueCreatedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.ISSUE_CREATED, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    public async Task SendIssueAssignedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.ISSUE_ASSIGNED, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    public async Task SendIssueDueDateUpdatedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.UPDATE_DUEDATE, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    public async Task SendIssuePriorityChangedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.UPDATE_PRIORITY, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    public async Task SendIssueStatusChangedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.UPDATE_STATUS, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    public async Task SendCommentAddedNotificationAsync(Issue issue)
    {
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.COMMENT_CREATED, issueDto);
        await sendEventToChatGpt(chatEvent);
    }

    private async Task sendEventToChatGpt(ChatGptDto chatEvent)
    {
        logger.LogDebug("Sending event to ChatGPT: {event} at URI: {uri}", chatEvent, _ChatServerUri);
        var response = await _httpClient.PostAsJsonAsync(_ChatServerUri, chatEvent, JsonOptions.Default);
        logger.LogDebug("Sent event {event} to chat server. Response status: {StatusCode}", chatEvent.Event, response.StatusCode);
    }
}
