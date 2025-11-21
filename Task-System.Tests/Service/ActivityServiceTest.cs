using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service;

public class ActivityServiceTests
{
    private PostgresqlDbContext GetInMemoryDb(string dbName = null)
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? "ActivityTestDb_" + Guid.NewGuid())
            .Options;
        return new PostgresqlDbContext(options);
    }

    [Fact]
    public async Task CreateActivityPropertyUpdatedAsync_ShouldCreateAndReturnActivity()
    {
        // Arrange
        var db = GetInMemoryDb();
        var mockLogger = new Mock<ILogger<ActivityService>>();
        var service = new ActivityService(db, mockLogger.Object);

        var issueId = 1;
        var oldValue = "Old";
        var newValue = "New";
        var type = ActivityType.UPDATED_ASSIGNEE;

        // Act
        var activity = await service.CreateActivityPropertyUpdatedAsync(type, oldValue, newValue, issueId);

        // Assert
        Assert.NotNull(activity);
        Assert.IsType<ActivityPropertyUpdated>(activity);
        Assert.Equal(oldValue, activity.OldValue);
        Assert.Equal(newValue, activity.NewValue);
        Assert.Equal(type, activity.Type);

        var dbActivity = await db.Activities.FirstOrDefaultAsync(a => a.Id == activity.Id);
        Assert.NotNull(dbActivity);
    }

    [Fact]
    public async Task CreateActivityPropertyCreatedAsync_ShouldCreateAndReturnActivity()
    {
        // Arrange
        var db = GetInMemoryDb();
        var mockLogger = new Mock<ILogger<ActivityService>>();
        var service = new ActivityService(db, mockLogger.Object);

        // Act
        var activity = await service.CreateActivityPropertyCreatedAsync(ActivityType.CREATED_ISSUE, 1);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(ActivityType.CREATED_ISSUE, activity.Type);
        Assert.Equal(1, activity.IssueId);

        var dbActivity = await db.Activities.FirstOrDefaultAsync();
        Assert.NotNull(dbActivity);
        Assert.Equal(activity.Id, dbActivity.Id);
        Assert.Single(await db.Activities.ToListAsync());
    }

}
