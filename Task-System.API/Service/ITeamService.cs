using Task_System.Model.DTO;
using Task_System.Model.Entity;
using Task_System.Model.Request;

namespace Task_System.Service;

public interface ITeamService
{
    Task<Team> GetTeamByIdAsync(int id);
    Task<List<Team>> GetAllTeamsAsync();
    Task<Team> AddTeamAsync(CreateTeamRequest req);

    Task<Team> GetTeamByName(string name);
    Task<List<IssueDto>> GetIssuesByTeamId(int teamId);
    Task<List<UserDto>> GetUsersByTeamId(int teamId);
}
