using Task_System.Model.IssueFolder;
using Task_System.Exception.ProjectException;
using Task_System.Data;
using Microsoft.EntityFrameworkCore;
namespace Task_System.Service.Impl;

public class ProjectService : IProjectService
{
    private readonly PostgresqlDbContext _db ;
    public async Task<Project> GetProjectById(int id)
    {
        Project? project = await _db.Projects.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (project == null) throw new ProjectNotFoundException($"Project id {id} was not found");
        return project;
    }

    public async Task<Project> GetProjectByName(string shortName)
    {
        Project? project = await _db.Projects.Where(p => p.ShortName == shortName).FirstOrDefaultAsync();
        if (project == null) throw new ProjectNotFoundException($"Project name {shortName} was not found");
        return project;
    }

    public ProjectService(PostgresqlDbContext db)
    {
        _db = db;
    }
}
