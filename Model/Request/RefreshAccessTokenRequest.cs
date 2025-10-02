namespace Task_System.Model.Request
{
    public record RefreshAccessTokenRequest(int UserId, string RefreshToken)
    {
    }
}
