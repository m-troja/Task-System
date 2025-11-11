using Task_System.Model.DTO;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(CreateCommentRequest ccr);
    Task<IEnumerable<CommentDto>> GetCommentsByIssueIdAsync(int issueId);
    Task DeleteAllCommentsByIssueId(int issueId);
    Task DeleteCommentById(int id);

}
