using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO;

public record IssueDto(int Id, string Title, string Description, IssueStatus status, int AuthorId, int AssigneeId, DateTime CreatedAt, DateTime? DueDate,
    DateTime? UpdatedAt, List<CommentDto> Comments, int ProjectId)
{ }

