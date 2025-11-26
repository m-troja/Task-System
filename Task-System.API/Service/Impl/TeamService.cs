using Microsoft.EntityFrameworkCore;
using Task_System.Data;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Service;

public class TeamService : ITeamService
{
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<TeamService> l;
    private readonly IssueCnv _issueCnv;
    private readonly UserCnv _userCnv;

    public TeamService(PostgresqlDbContext db, ILogger<TeamService> l, IssueCnv issueCnv, UserCnv userCnv)
    {
        _db = db;
        this.l = l;
        _issueCnv = issueCnv;
        _userCnv = userCnv;
    }

    private void ValidateTeamName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            l.LogDebug("Team name is null or empty");
            throw new ArgumentException("Team name cannot be null or empty");
        }
    }

    public async Task<List<Team>> GetAllTeamsAsync()
    {
        l.LogDebug("Getting all teams from the db");
        return await _db.Teams.ToListAsync();
    }

    public async Task<Team> GetTeamByIdAsync(int id)
    {
        l.LogDebug($"Getting team by id: {id}");
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == id);
        if (team == null)
        {
            l.LogDebug($"Team with id {id} not found");
            throw new KeyNotFoundException($"Team with id {id} not found");
        }
        return team;
    }

    public async Task<Team> GetTeamByName(string name)
    {
        ValidateTeamName(name);
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Name == name);
        if (team == null)
        {
            l.LogDebug($"Team with name {name} was not found");
            throw new KeyNotFoundException($"Team with name {name} was not found");
        }
        return team;
    }

    public async Task<Team> AddTeamAsync(CreateTeamRequest req)
    {
        ValidateTeamName(req.Name);

        var existingTeam = await _db.Teams.FirstOrDefaultAsync(t => t.Name == req.Name);
        if (existingTeam != null)
        {
            l.LogDebug($"Team with name {req.Name} already exists");
            throw new ArgumentException($"Team with name {req.Name} already exists");
        }

        var newTeam = new Team(req.Name);
        await _db.Teams.AddAsync(newTeam);
        await _db.SaveChangesAsync();
        return newTeam;
    }

    public async Task<List<IssueDto>> GetIssuesByTeamId(int teamId)
    {
        var team = await GetTeamByIdAsync(teamId);
        var issues = team.Issues?.ToList() ?? new List<Issue>();
        l.LogDebug($"Found {issues.Count} issues in team with id {teamId}");
        return _issueCnv.ConvertIssueListToIssueDtoList(issues);
    }

    public async Task<List<UserDto>> GetUsersByTeamId(int teamId)
    {
        var team = await GetTeamByIdAsync(teamId);
        var users = team.Users?.ToList() ?? new List<User>();
        l.LogDebug($"Found {users.Count} users in team with id {teamId}");
        return _userCnv.ConvertUsersToUsersDto(users);
    }
}