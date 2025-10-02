using Task_System.Model.Entity;

namespace Task_System.Model.DTO
{
    public record UserDto(int Id, string FirstName, string LastName, string Email, ICollection<string> Roles, List<string> Teams)
    {
    }
}
