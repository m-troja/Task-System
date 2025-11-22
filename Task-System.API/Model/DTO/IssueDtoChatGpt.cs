using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO;

public record IssueDtoChatGpt(
    int Id, 
    string Key, 
    string Title, 
    string Description, 
    IssueStatus Status, 
    IssuePriority Priority,
    string AuthorSlackId,
    string AssigneeSlackId, 
    DateTime CreatedAt, 
    DateTime? DueDate,
    DateTime? UpdatedAt, 
    ICollection<CommentDto> Comments, 
    int ProjectId)
{ }

