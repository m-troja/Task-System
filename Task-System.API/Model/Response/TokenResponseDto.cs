using Task_System.Model.Entity;

namespace Task_System.Model.Response;

public record TokenResponseDto(
    string AccessToken,
    RefreshToken RefreshToken)
{}