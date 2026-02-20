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
    private readonly HttpClient _httpClient;
    public SlackNotificationService(ILogger<SlackNotificationService> logger, IssueCnv _issueCnv, HttpClient _httpClient)

    {
        this._httpClient = _httpClient;
        this.logger = logger;
        this._issueCnv = _issueCnv;
    }
    public async Task SendIssueCreatedNotificationAsync(Issue issue)
    {
        string uri = "http://" +  _ChatServerAddress + ":" + _ChatServerPort + _EventEndpoint;
        var issueDto = _issueCnv.ConvertIssueToIssueDtoChatGpt(issue);
        var chatEvent = new ChatGptDto(ChatGptEvent.ISSUE_CREATED, issueDto);


        logger.LogDebug("SendIssueAssignedNotificationAsync URI: {uri}, issueDto: {dto}", uri, chatEvent);
        var response = _httpClient.PostAsJsonAsync(uri, chatEvent, JsonOptions.Default);
        logger.LogDebug("Sent issue assigned notification for issue {IssueId} to chat server. Response status: {StatusCode}", issue.Id, response.Result.StatusCode);

    }

    public Task SendIssueAssignedNotificationAsync(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task SendIssueDueDateUpdatedNotificationAsync(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task SendIssuePriorityChangedNotificationAsync(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task SendIssueStatusChangedNotificationAsync(Issue issue)
    {
        throw new NotImplementedException();
    }
}
