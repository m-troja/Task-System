namespace Task_System.Model.IssueFolder;

public class Key
{
    public int Id { get; set; }
    public string KeyString { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Issue Issue { get; set; } = null!;
    public int ProjectId { get; set; }  
    public int IssueId { get; set; }

    public Key(Project project, Issue issue)
    {
        Project = project;
        Issue = issue;
        ProjectId = project.Id;
        IssueId = issue.Id;
        KeyString = $"{project.ShortName}-{issue.Id}"; // e.g., JAVA-123
    }

    public Key()
    {
    }
}
