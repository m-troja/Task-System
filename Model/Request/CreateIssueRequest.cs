namespace Task_System.Model.Request
{
    public record CreateIssueRequest(string Title, string? Description, string? Priority, int AuthorId, int? AssigneeId, string? DueDate )
    {
    }
}
