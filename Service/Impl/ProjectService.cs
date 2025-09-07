using Task_System.Model.IssueFolder;
using Task_System.Exception.ProjectException;
using Task_System.Data;
using Microsoft.EntityFrameworkCore;
using Task_System.Model.Request;
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

    public async Task<Project> CreateProject(CreateProjectRequest cpr)
    {
        if (cpr.description == null) cpr = cpr with { description = "" };

        Project newProject = new Project(cpr.shortName, cpr.description);

        _db.Projects.Add(newProject);
        await _db.SaveChangesAsync();

        return newProject;
    }

    public ProjectService(PostgresqlDbContext db)
    {
        _db = db;
    }
}
