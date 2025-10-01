using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.DTO;

namespace Task_System.Service
{
    public interface IIssueService
    {
        Task<Issue> GetIssueByIdAsync(int id);
        Task<IssueDto> GetIssueDtoByIdAsync(int id);
        Task<IssueDto> GetIssueDtoByKeyAsync(string keyString);
        Task<IEnumerable<Issue>> GetAllAsync();
        Task<Issue> CreateIssueAsync(CreateIssueRequest cir);
        Task<bool> DeleteIssueAsync(int id);
        Task<IssueDto> AssignIssueAsync(AssignIssueRequest uir);
        Task<Issue> UpdateIssueAsync(Issue issue);
        Task<Project> GetProjectFromKey(string key);
        int GetIssueIdInsideProjectFromKey(string key);
        Task<IssueDto> RenameIssueAsync(RenameIssueRequest rir);
        Task<IssueDto> ChangeIssueStatusAsync(ChangeIssueStatusRequest req);
        Task<IssueDto> ChangeIssuePriorityAsync(ChangeIssuePriorityRequest req);
        Task<IssueDto> AssignTeamAsync(AssignTeamRequest req);
        Task<IEnumerable<IssueDto>> GetAllIssuesByUserId(int userId);
        Task<IEnumerable<IssueDto>> GetAllIssuesByProjectId(int projectId);
        Task<int> GetIssueIdFromKey(string key);

    }
}
