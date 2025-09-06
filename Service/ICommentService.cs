using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface ICommentService
{
    Task<Comment> CreateCommentAsync(CreateCommentRequest ccr);
    Task<IEnumerable<Comment>> GetCommentsByIssueIdAsync(int issueId);
}
