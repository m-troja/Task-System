using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_System.Data;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service;

public class TeamServiceTests
{
    private PostgresqlDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PostgresqlDbContext(options);
    }

    private TeamService CreateService(PostgresqlDbContext db)
    {
        var mockLogger = new Mock<ILogger<TeamService>>();
        var issuesCnvLogger = new LoggerFactory().CreateLogger<IssueCnv>();
        var commentCnv = new CommentCnv();
        var teamCnvLogger = new LoggerFactory().CreateLogger<TeamCnv>();
        var teamCnv = new TeamCnv(teamCnvLogger);
        var loggerIssueCnv = new LoggerFactory().CreateLogger<IssueCnv>();
        var issueCnv = new IssueCnv(commentCnv, loggerIssueCnv, teamCnv);
        var userCnv = new UserCnv();
        return new TeamService(db, mockLogger.Object, issueCnv, userCnv);
    }

    [Fact]
    public async Task GetAllTeamsAsync_ShouldReturnAllTeams()
    {
        var db = GetDb();
        db.Teams.Add(new Team("Team1"));
        db.Teams.Add(new Team("Team2"));
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var teams = await service.GetAllTeamsAsync();

        Assert.Equal(2, teams.Count);
        Assert.Contains(teams, t => t.Name == "Team1");
        Assert.Contains(teams, t => t.Name == "Team2");
    }

    [Fact]
    public async Task GetTeamByIdAsync_ShouldReturnTeam_WhenExists()
    {
        var db = GetDb();
        var team = new Team("Team1");
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var result = await service.GetTeamByIdAsync(team.Id);

        Assert.Equal(team.Name, result.Name);
    }

    [Fact]
    public async Task GetTeamByIdAsync_ShouldThrow_WhenNotFound()
    {
        var db = GetDb();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTeamByIdAsync(999));
    }

    [Fact]
    public async Task AddTeamAsync_ShouldAddTeam_WhenNameValid()
    {
        var db = GetDb();
        var service = CreateService(db);

        var request = new CreateTeamRequest("Team1");

        var result = await service.AddTeamAsync(request);

        var teamInDb = await db.Teams.FirstOrDefaultAsync(t => t.Name == "Team1");
        Assert.NotNull(teamInDb);
        Assert.Equal("Team1", result.Name);
        Assert.Equal(teamInDb.Id, result.Id);
    }

    [Fact]
    public async Task AddTeamAsync_ShouldThrowArgumentException_WhenNameEmpty()
    {
        var db = GetDb();
        var service = CreateService(db);

        var request = new CreateTeamRequest("");

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddTeamAsync(request));
    }

    [Fact]
    public async Task AddTeamAsync_ShouldThrow_WhenNameExists()
    {
        var db = GetDb();
        db.Teams.Add(new Team("Existing"));
        await db.SaveChangesAsync();

        var service = CreateService(db);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddTeamAsync(new CreateTeamRequest("Existing")));
    }

    [Fact]
    public async Task GetTeamByName_ShouldReturnTeam_WhenExists()
    {
        var db = GetDb();
        var team = new Team("MyTeam");
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var result = await service.GetTeamByName("MyTeam");

        Assert.Equal(team.Name, result.Name);
    }

    [Fact]
    public async Task GetTeamByName_ShouldThrow_WhenNotFound()
    {
        var db = GetDb();
        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetTeamByName("NoTeam"));
    }

    [Fact]
    public async Task GetIssuesByTeamId_ShouldReturnIssues()
    {
        var commentCnv = new CommentCnv();
        var teamCnvLogger = new LoggerFactory().CreateLogger<TeamCnv>();
        var teamCnv = new TeamCnv(teamCnvLogger);
        var loggerIssueCnv = new LoggerFactory().CreateLogger<IssueCnv>();
        var issueCnv = new IssueCnv(commentCnv, loggerIssueCnv, teamCnv);

        // Arrange
        var db = GetDb();
        var team = new Team("Team1");
        var issue = new Issue
        {
            Title = "Issue1",
            Key = new Key { KeyString = "ISSUE-1" },
            Status = IssueStatus.NEW,
            Priority = IssuePriority.NORMAL,
            CreatedAt = DateTime.UtcNow,
            ProjectId = 1
        };
        team.Issues.Add(issue);
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var mockLogger = new Mock<ILogger<TeamService>>();
        var service = new TeamService(
            db,
            mockLogger.Object,
            new IssueCnv(commentCnv, loggerIssueCnv, teamCnv),
            new UserCnv()
        );

        // Act
        var result = await service.GetIssuesByTeamId(team.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal("Issue1", result[0].Title);
        Assert.Equal("ISSUE-1", result[0].Key);
    }


    [Fact]
    public async Task GetUsersByTeamId_ShouldReturnUsers()
    {
        // Arrange
        var db = GetDb();
        var team = new Team("Team1");
        var user = new User("John", "Doe", "john@example.com", "pw", new byte[16], new Role("USER"));
        team.Users.Add(user);
        db.Teams.Add(team);
        await db.SaveChangesAsync();
        
        var mockLogger = new Mock<ILogger<TeamService>>();
        var service = CreateService(db);

        // Act
        var result = await service.GetUsersByTeamId(team.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal("john@example.com", result[0].Email);
        Assert.Contains("USER", result[0].Roles);
    }
}
