using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Task_System.Data;
using Task_System.Exception.ProjectException;
using Task_System.Model.IssueFolder;
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
        if (cpr.description.Length > 255) throw new InvalidProjectData("Project short name cannot be longer than 255 characters");
       
        string shortNameTrimmed = cpr.shortName.Trim().ToUpper();
        if (shortNameTrimmed.Length == 0 || string.IsNullOrWhiteSpace(cpr.description)) throw new InvalidProjectData("Project short name cannot be empty or whitespace only");
        if (shortNameTrimmed.Length > 6) throw new InvalidProjectData("Project short name cannot be longer than 6 characters");
        if (!Regex.IsMatch(cpr.shortName, @"^[a-zA-Z\s]+$")) throw new InvalidProjectData("Project shortName can contain only letters and spaces");
       
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
