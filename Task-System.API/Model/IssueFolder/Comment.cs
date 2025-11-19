using Task_System.Model.Entity;

namespace Task_System.Model.IssueFolder;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int AuthorId { get; set; }      // FK
    public int IssueId { get; set; }       // FK
    public DateTime CreatedAt { get; set; }   // UTC
    public User Author { get; set; }
    public Issue Issue { get; set; }
    public Comment(string content, User author, Issue issue)
    {
        Content = content;
        Author = author;
        Issue = issue;
    }
    public Comment() { }
}
