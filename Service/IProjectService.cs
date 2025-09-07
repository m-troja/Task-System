using Task_System.Model.IssueFolder;

namespace Task_System.Service;

public interface IProjectService
{
    public Task<Project> GetProjectByName(string shortName);
    public Task<Project> GetProjectById(int id);
}
