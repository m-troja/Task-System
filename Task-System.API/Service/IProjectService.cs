using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface IProjectService
{
    public Task<Project> GetProjectByName(string shortName);
    public Task<Project> GetProjectById(int id);
    public Task DeleteProjectById(int id);

    public Task<Project> CreateProject(CreateProjectRequest cpr);
    public Task<IEnumerable<Project>> GetAllProjectsAsync();
}
