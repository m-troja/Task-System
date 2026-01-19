namespace Task_System.Model.DTO;

public record TeamDto(
    int Id, 
    string Name, 
    List<int> Issues, 
    List<UserDto> users)
{
}
