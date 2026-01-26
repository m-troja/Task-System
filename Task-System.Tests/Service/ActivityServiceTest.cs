using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Service.Impl;
using Xunit;
namespace Task_System.Tests.Service;

public class ActivityServiceTests
{
    private static PostgresqlDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new PostgresqlDbContext(options);

        db.Activities.RemoveRange(db.Activities);
        db.SaveChanges();

        return db;
    }

    private static ActivityService CreateService(PostgresqlDbContext db)
    {
        var logger = new Mock<ILogger<ActivityService>>();
        return new ActivityService(db, logger.Object);
    }

    [Fact]
    public async Task CreateActivityPropertyCreatedAsync_ShouldCreateAndPersistActivity()
    {
        var logger = new Mock<ILogger<ActivityService>>();
        await using var db = CreateInMemoryDbContext();
        var service = CreateService(db);

        var type = ActivityType.CREATED_COMMENT;
        var issueId = 42;

        var result = await service.CreateActivityPropertyCreatedAsync(type, issueId);

        Assert.NotNull(result);
        Assert.Equal(type, result.Type);
        Assert.Equal(issueId, result.IssueId);

        var fromDb = await db.Activities.OfType<ActivityPropertyCreated>().SingleAsync();
        Assert.Equal(result.Id, fromDb.Id);
    }


    [Fact]
    public async Task CreateActivityPropertyUpdatedAsync_ShouldCreateAndPersistActivity()
    {
        // arrange
        await using var db = CreateInMemoryDbContext();
        var service = CreateService(db);

        var type = ActivityType.UPDATED_STATUS;
        var oldValue = "OPEN";
        var newValue = "CLOSED";
        var issueId = 99;

        // act
        var result = await service.CreateActivityPropertyUpdatedAsync(type, oldValue, newValue, issueId);

        // assert
        Assert.Equal(type, result.Type);
        Assert.Equal(oldValue, result.OldValue);
        Assert.Equal(newValue, result.NewValue);
        Assert.Equal(issueId, result.IssueId);

        var fromDb = await db.Activities
            .OfType<ActivityPropertyUpdated>()
            .SingleAsync();

        Assert.Equal(result.Id, fromDb.Id);
    }
}
