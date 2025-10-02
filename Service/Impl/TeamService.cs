using Microsoft.EntityFrameworkCore;
using Task_System.Log;
using Task_System.Data;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service.Impl;

public class TeamService : ITeamService
{
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<TeamService> l;
    private readonly IssueCnv _issueCnv;
    private readonly UserCnv _userCnv;

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
            l.log($"Team with name {name} was not found");
            throw new KeyNotFoundException($"Team with name {name} was not found");
        }
        return team;
    }

    public async Task<List<IssueDto>> GetIssuesByTeamId(int teamId)
    {
        var Team = await GetTeamByIdAsync(teamId);
        List<Issue> issues = Team.Issues.ToList();
        l.log($"Found {issues.Count} issues in team with id {teamId}");
        return _issueCnv.ConvertIssueListToIssueDtoList(issues);
    }

    public async Task<List<UserDto>> GetUsersByTeamId(int teamId)
    {
        var Team = await GetTeamByIdAsync(teamId);
        List<User> users = Team.Users.ToList();
        l.log($"Found {users.Count} users in team with id {teamId}");
        return _userCnv.ConvertUsersToUsersDto(users);
    }

    public TeamService(PostgresqlDbContext db, ILogger<TeamService> l, IssueCnv issueCnv, UserCnv userCnv)
    {
        _db = db;
        this.l = l;
        _issueCnv = issueCnv;
        _userCnv = userCnv;
    }
}
