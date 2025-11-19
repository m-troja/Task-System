namespace Task_System.Model.Request;

public record UpdateDueDateRequest(int IssueId, DateTime? DueDate)
{
}
