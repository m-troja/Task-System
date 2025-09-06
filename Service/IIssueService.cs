using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service
{
    public interface IIssueService
    {
        Task<Model.IssueFolder.Issue> GetByIdAsync(int id);
        Task<IEnumerable<Issue>> GetAllAsync();
        Task<Issue> CreateIssueAsync(CreateIssueRequest cir);
        Task<Issue> UpdateIssueAsync(Issue issue);
        Task<bool> DeleteIssueAsync(int id);
    }
}
