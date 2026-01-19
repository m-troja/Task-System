using Task_System.Model.Entity;
using Task_System.Log;
namespace Task_System.Model.DTO.Cnv;

public class TeamCnv
{
    private readonly IssueCnv _issueCnv;
    private readonly ILogger<TeamCnv> l;
    private readonly UserCnv _userCnv;
    public TeamDto ConvertTeamToTeamDto(Team team)
    {
        l.LogDebug($"Converting Team entity to TeamDto: {team}");
        var NewTeamDto =  new TeamDto
       (
           team.Id,
           team.Name,
           team.Issues?.Select(i => i.Id).ToList() ?? new List<int>(),
           team.Users?.Select( u => _userCnv.ConvertUserToDto(u)).ToList() ?? new List<UserDto>()
        );
        l.LogDebug($"Converted TeamDto: {NewTeamDto}");
        return NewTeamDto;
    }

    public IEnumerable<TeamDto> ConvertTeamsToTeamDtos(ICollection<Team> teams)
    {
        var teamDtos = new List<TeamDto>();
        foreach (var team in teams)
        {
            teamDtos.Add(ConvertTeamToTeamDto(team));
        }
        return teamDtos;
    }

    public TeamCnv(IssueCnv issueCnv, ILogger<TeamCnv> logger, UserCnv userCnv)
    {
        _issueCnv = issueCnv;
        l = logger;
        _userCnv = userCnv;
    }
}
