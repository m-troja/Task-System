namespace Task_System.Model.DTO;

public record TeamDto(int Id, string Name, List<IssueDto> Issues, List<UserDto> users)
{
}
