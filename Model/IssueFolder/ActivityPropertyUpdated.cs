using Task_System.Model.Entity;

namespace Task_System.Model.IssueFolder;

public class ActivityPropertyUpdated : Activity
{
    public int FromId { get; set; }
    public int ToId { get; set; }

    public ActivityPropertyUpdated(int fromId, int toId, int IssueId, ActivityType Type) : base(Type, IssueId)
    {
        this.FromId = fromId;
        this.ToId = toId;
        this.Type = Type;
        this.IssueId = IssueId;
    }
}
