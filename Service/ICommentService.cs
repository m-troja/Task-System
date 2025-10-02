using Task_System.Model.DTO;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(CreateCommentRequest ccr);
    Task<IEnumerable<Comment>> GetCommentsByIssueIdAsync(int issueId);
}
