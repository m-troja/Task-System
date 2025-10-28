namespace Task_System.Model.Request;

public record SlackRegistrationRequest(
    string slackName, 
    string slackUserId)
{}