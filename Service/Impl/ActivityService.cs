using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Config;
namespace Task_System.Service.Impl;

public class ActivityService : IActivityService
{
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<ActivityService> l;
    public async Task NewAssignIssueActivity(User oldAssignee, Issue issue)
    {
        l.log($"Logging new assign issue activity for {issue}, old assignee {oldAssignee}");

        string text = issue.Assignee == null 
            ? $"Issue unassigned from {oldAssignee.FirstName} {oldAssignee.LastName}" 
            : $"Issue assigned from {oldAssignee.FirstName} {oldAssignee.LastName} to {issue.Assignee.FirstName} {issue.Assignee.LastName}";
        User author = issue.Assignee == null ? oldAssignee : issue.Assignee;

        // No new assignee, no activity to log
        var activity = new Activity(
          ActivityType.TICKET_ASSIGNED,
          text,
          author,
          issue
        );
        await _db.Activities.AddAsync(activity);
        await _db.SaveChangesAsync();
    }

    public Task NewCreateIssueActivity(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task NewSetDueDateActivity(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task NewSetPriorityActivity(Issue issue)
    {
        throw new NotImplementedException();
    }

    public Task NewSetStatusActivity(Issue issue)
    {
        throw new NotImplementedException();
    }

    public ActivityService(PostgresqlDbContext db, ILogger<ActivityService> l)
    {
        _db = db;
        this.l = l;
    }
}
