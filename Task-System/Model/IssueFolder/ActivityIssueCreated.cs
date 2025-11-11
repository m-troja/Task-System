using Task_System.Model.Entity;

namespace Task_System.Model.IssueFolder;

public class ActivityPropertyCreated : Activity
{
    public ActivityPropertyCreated(ActivityType Type, int issueId) : base(Type, issueId)
    {
    }
}
