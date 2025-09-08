namespace Task_System.Model.Entity;

public abstract class Activityable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // UTC
    public User Author { get; set; } = null!;

    protected Activityable(User author)
    {
        Author = author;
    }
}
