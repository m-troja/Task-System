namespace Task_System.Model.Entity;

public class RefreshToken
{
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
}