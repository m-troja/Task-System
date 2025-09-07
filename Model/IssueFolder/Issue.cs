using Task_System.Model.Entity;
using Task_System.Service;

namespace Task_System.Model.IssueFolder
{
    public class Issue : IAutomaticDates
    {
        public int Id { get; set; }
        public int IdInsideProject { get; set; } = 0!;
        public int ProjectId { get; set; } = 0!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int AuthorId { get; set; }      // FK
        public int? AssigneeId { get; set; }   // FK
        public DateTime CreatedAt { get; set; }   // UTC
        public DateTime? UpdatedAt { get; set; }  // UTC
        public DateTime? DueDate { get; set; }    // UTC
        public IssueStatus Status { get; set; } = IssueStatus.New;
        public IssuePriority? Priority { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public User Author { get; set; } = null!;
        public User? Assignee { get; set; }
        public Project? Project { get; set; }
        public Key Key { get; set; } = null!;

        public Issue(string title, User author)
        {
            Title = title;
            Author = author;
        }

        public Issue(string title, string? description, IssuePriority? priority,  User author, User? assignee, DateTime? dueDate, 
            int authorId, int? assigneeId, int projectId, int idInsideProject )
        {
            Title = title;
            Description = description;
            Author = author;
            Assignee = assignee;
            DueDate = dueDate;
            Priority = priority;
            AuthorId = authorId;
            AssigneeId = assigneeId;
            ProjectId = projectId;
            IdInsideProject = idInsideProject;
        }
        public Issue() { }

    }
}
