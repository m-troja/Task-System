namespace Task_System.Model.Request;

public record ChangeIssueStatusRequest(int IssueId, string NewStatus)
{
}
