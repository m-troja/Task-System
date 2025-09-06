using Task_System.Model.Entity;

namespace Task_System.Model.DTO
{
    public record UserDto(string FirstName, string LastName, string Email, List<string> Roles, List<string> Teams)
    {
    }
}
