using Task_System.Model.Entity;

namespace Task_System.Model.IssueFolder
{
    public class Issue
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required User Author { get; set; }
        public User? Assignee { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required IssueStatus Status { get; set; }
        public IssuePriority? Priority { get; set; }
    
        public Issue(string title, User author)
        {
            Title = title;
            Author = author;
            CreatedAt = DateTime.Now;
            Status = IssueStatus.New;
        }
    }
}
