using Task_System.Service;

namespace Task_System.Model.IssueFolder;

public class Project 
{
    public int Id { get; set; }
    public string ShortName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get ; set; } = DateTime.UtcNow;
    public ICollection<Issue>? Issues { get; set; } = new List<Issue>();
    public ICollection<Key>? Keys { get; set; } = new List<Key>();  
    public Project(string shortName)
    {
        ShortName = shortName;
    }

    public Project()
    {
    }
}
