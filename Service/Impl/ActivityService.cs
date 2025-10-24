using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Log;
namespace Task_System.Service.Impl;

public class ActivityService : IActivityService
{
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<ActivityService> l;

    public async Task<ActivityPropertyUpdated> CreateActivityPropertyUpdatedAsync(ActivityType Type, int fromId, int toId, int issueId)
    {
        l.log($"Creating ActivityPropertyUpdated: Type={Type}, fromId={fromId}, toId={toId}, issueId={issueId}");
        var activity = new ActivityPropertyUpdated(fromId, toId, issueId, Type);
        _db.Activities.Add(activity);
        await _db.SaveChangesAsync();
        return activity;
    }


    public ActivityService(PostgresqlDbContext db, ILogger<ActivityService> l)
    {
        _db = db;
        this.l = l;
    }
}
