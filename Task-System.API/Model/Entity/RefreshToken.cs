namespace Task_System.Model.Entity;

public record RefreshToken
(
    string Token,
    int UserId,
    DateTime Expires,
    bool IsRevoked
){}