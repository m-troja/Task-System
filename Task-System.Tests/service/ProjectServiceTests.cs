using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task_System.Data;
using Task_System.Model;
using Task_System.Model.Request;
using Task_System;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests;

public class ProjectServiceTests
{
    private PostgresqlDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase("TestDb")
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
}
