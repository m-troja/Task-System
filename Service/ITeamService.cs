using Task_System.Model.Entity;

namespace Task_System.Service;

public interface ITeamService
{
    Task<Team> GetTeamByIdAsync(int id);
    Task<IEnumerable<Team>> GetAllTeamsAsync();
}
