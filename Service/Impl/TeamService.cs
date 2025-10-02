using Microsoft.EntityFrameworkCore;
using Task_System.Config;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Model.Request;

namespace Task_System.Service.Impl;

public class TeamService : ITeamService
{
private readonly PostgresqlDbContext _db;
private readonly ILogger<TeamService> l;

    public async Task<List<Team>> GetAllTeamsAsync()
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

    public async Task<Team> AddTeamAsync(CreateTeamRequest req)
    {
        if (req.Name == null || req.Name.Trim() == "")
        {
            l.log("Team name is null or empty");
            throw new ArgumentException("Team name cannot be null or empty");
        }
        Team team; 
        try
        {
            team = await GetTeamByName(req.Name);

        }
        catch (KeyNotFoundException)
        {
            team = null;
        }

        if (team != null)
        {
            l.log($"Team with name {req.Name} already exists");
            throw new ArgumentException($"Team with name {req.Name} already exists");
        }   
        var NewTeam = new Team
        {
            Name = req.Name,
        };
        await _db.Teams.AddAsync(NewTeam);
        await _db.SaveChangesAsync();
        return NewTeam;
    }

    public async Task<Team> GetTeamByName(string name)
    {
        if (name == null || name.Trim() == "")
        {
            l.log("Team name is null or empty");
            throw new ArgumentException("Team name cannot be null or empty");
        }
        Team team = await _db.Teams.FirstOrDefaultAsync(t => t.Name == name);
        if (team == null)
        {
            l.log($"Team with name {name} not found");
            throw new KeyNotFoundException($"Team with name {name} not found");
        }
        return team;
    }

    public TeamService(PostgresqlDbContext db, ILogger<TeamService> l)
    {
        _db = db;
        this.l = l;
    }
}
