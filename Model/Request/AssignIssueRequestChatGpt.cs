namespace Task_System.Model.Request;

public record AssignIssueRequestChatGpt(
    string key, 
    string slackUserId)
{}
