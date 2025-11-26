using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;

namespace Task_System.Service;

public interface IActivityService
{
    public Task<ActivityPropertyUpdated> CreateActivityPropertyUpdatedAsync(ActivityType Type, string OldValue, string NewValue, int issueId);
    public Task<ActivityPropertyCreated> CreateActivityPropertyCreatedAsync(ActivityType Type, int issueId);

}
