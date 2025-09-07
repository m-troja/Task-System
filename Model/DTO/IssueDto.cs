using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO;

public record IssueDto(string key, string Title, string Description, IssueStatus status, int AuthorId, int AssigneeId, DateTime CreatedAt, DateTime? DueDate,
    DateTime? UpdatedAt, ICollection<CommentDto> Comments, int ProjectId)
{ }

