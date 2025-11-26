namespace Task_System.Model.DTO;

public record RefreshTokenDto (
    string Token,
    DateTime Expires
)
{}
