using Task_System.Model.IssueFolder;

namespace Task_System.Model.Entity;

public class Activity : Activityable
{
    public int Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public string Text { get; set; } = null!;
    public int IssueId { get; set; } // FK
    public Issue Issue { get; set; } = null!;

    public Activity(ActivityType activityType, string text, User author, Issue issue) : base(author)
    {
        ActivityType = activityType;
        Text = text;
        Issue = issue;
        IssueId = issue.Id;
    }

    public Activity() : base(null!) { }
}
