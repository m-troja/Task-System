using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task_System.Data;
using Task_System.Model;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service;

public class ProjectServiceTests
{
    private PostgresqlDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PostgresqlDbContext(options);
    }

    private ILogger<ProjectService> GetLoggerStub() =>
        new LoggerFactory().CreateLogger<ProjectService>();

    [Fact]
    public async Task CreateProject_ShouldAddProject()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var service = new ProjectService(db, logger);

        var request = new CreateProjectRequest("TEST", "Test project");
        var project = await service.CreateProject(request);

        Assert.NotNull(project);
        Assert.Equal("TEST", project.ShortName);
        Assert.Equal("Test project", project.Description);
        Assert.Single(await db.Projects.ToListAsync());
    }

    [Fact]
    public async Task<Project> GetProjectById_ShouldReturnProject()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var service = new ProjectService(db, logger);

        var project = new Model.IssueFolder.Project(2, "TEST2", "Test project2", DateTime.UtcNow);
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        var fetchedProject = await service.GetProjectById(project.Id);
        Assert.NotNull(fetchedProject);
        Assert.Equal(project.Id, fetchedProject.Id);
        return fetchedProject;
    }
}
