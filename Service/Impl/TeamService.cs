using Microsoft.EntityFrameworkCore;
using Task_System.Config;
using Task_System.Data;
using Task_System.Model.Entity;

namespace Task_System.Service.Impl;

public class TeamService : ITeamService
{
private readonly PostgresqlDbContext _db;
private readonly ILogger<TeamService> l;

    public async Task<IEnumerable<Team>> GetAllTeamsAsync()
    {
        l.log("Getting all teams from the db");
        return await _db.Teams.ToListAsync();
    }

    public async Task<Team> GetTeamByIdAsync(int id)
    {
        l.log($"Getting team by id: {id}");
        var Team = await _db.Teams.FirstAsync(t => t.Id == id);
        if (Team == null)
        {
            l.log($"Team with id {id} not found");
            throw new KeyNotFoundException($"Team with id {id} not found");
        }
        return Team;
    }

    public TeamService(PostgresqlDbContext db, ILogger<TeamService> l)
    {
        _db = db;
        this.l = l;
    }
}
