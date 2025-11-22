using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task_System.Controller;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Service;
using Xunit;

namespace Task_System.Tests.Controller
{
    public class TeamControllerTest
    {
        private TeamController CreateController(
            Mock<IUserService> userService,
            Mock<ITeamService> teamService)
        {
            ILogger<IssueCnv> mockIssueService = new LoggerFactory().CreateLogger<IssueCnv>();
            var commentCnv = new CommentCnv();
            var issueCnv = new IssueCnv(commentCnv, mockIssueService);
            ILogger<TeamCnv> teamCnvLogger = new LoggerFactory().CreateLogger<TeamCnv>();
            var userCnv = new UserCnv();
            var teamCnv = new TeamCnv(issueCnv, teamCnvLogger, userCnv);
            var logger = new LoggerFactory().CreateLogger<TeamController>();
            return new TeamController(userService.Object, teamCnv, logger, teamService.Object);
        }

        [Fact]
        public async Task GetTeamById_ShouldReturnTeam_WhenTeamExists()
        {
            // GIVEN
            var teamServiceMock = new Mock<ITeamService>();
            var userServiceMock = new Mock<IUserService>();
            var team = new Team("Team A") { Id = 1 };

            teamServiceMock.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);
            var controller = CreateController(userServiceMock, teamServiceMock);

            // WHEN
            var result = await controller.GetTeamById(1);

            // THEN
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TeamDto>(okResult.Value);
            Assert.Equal(team.Id, returnedDto.Id);
            Assert.Equal(team.Name, returnedDto.Name);
        }

        [Fact]
        public async Task GetAllTeams_ShouldReturnListOfTeams()
        {
            // GIVEN
            var teamServiceMock = new Mock<ITeamService>();
            var userServiceMock = new Mock<IUserService>();
            var teams = new List<Team>
            {
                new Team("Team A") { Id = 1 },
                new Team("Team B") { Id = 2 }
            };

            teamServiceMock.Setup(s => s.GetAllTeamsAsync()).ReturnsAsync(teams);
            var controller = CreateController(userServiceMock, teamServiceMock);

            // WHEN
            var result = await controller.GetAllTeams();

            // THEN
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsType<List<TeamDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
            Assert.Equal("Team A", returnedList[0].Name);
            Assert.Equal("Team B", returnedList[1].Name);
        }

        [Fact]
        public async Task CreateTeam_ShouldReturnCreatedTeam()
        {
            // GIVEN
            var teamServiceMock = new Mock<ITeamService>();
            var userServiceMock = new Mock<IUserService>();

            var request = new CreateTeamRequest("Team C");
            var team = new Team("Team C") { Id = 3 };

            teamServiceMock.Setup(s => s.AddTeamAsync(request)).ReturnsAsync(team);
            var controller = CreateController(userServiceMock, teamServiceMock);

            // WHEN
            var result = await controller.CreateTeam(request);

            // THEN
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TeamDto>(okResult.Value);
            Assert.Equal(team.Id, returnedDto.Id);
            Assert.Equal(team.Name, returnedDto.Name);
        }

        [Fact]
        public async Task GetIssuesByTeamId_ShouldReturnListOfIssues()
        {
            // GIVEN
            var teamServiceMock = new Mock<ITeamService>();
            var userServiceMock = new Mock<IUserService>();

            var issues = BuildListOfIssueDto();
            teamServiceMock.Setup(s => s.GetIssuesByTeamId(1)).ReturnsAsync(issues);

            var controller = CreateController(userServiceMock, teamServiceMock);

            // WHEN
            var result = await controller.GetIssuesByTeamId(1);

            // THEN
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedIssues = Assert.IsType<List<IssueDto>>(okResult.Value);
            Assert.Equal(2, returnedIssues.Count);
        }

        [Fact]
        public async Task GetUsersByTeamId_ShouldReturnListOfUsers()
        {
            // GIVEN
            var teamServiceMock = new Mock<ITeamService>();
            var userServiceMock = new Mock<IUserService>();

            var users = new List<UserDto>
            {
                new UserDto(1, "John", "Doe", "a@test.com", new List<string>(), new List<string>(), false, "SLACK1"),
                new UserDto(2, "Jane", "Doe", "b@test.com", new List<string>(), new List<string>(), false, "SLACK2")
            };

            teamServiceMock.Setup(s => s.GetUsersByTeamId(1)).ReturnsAsync(users);

            var controller = CreateController(userServiceMock, teamServiceMock);

            // WHEN
            var result = await controller.GetUsersByTeamId(1);

            // THEN
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        private List<IssueDto> BuildListOfIssueDto()
        {
            var issue1 = new IssueDto(1, "ISSUE-1", "Title1", "Desc1", IssueStatus.NEW,
                        IssuePriority.HIGH, 1, 2, DateTime.Parse("2025-11-22"), DateTime.Parse("2025-11-23"), DateTime.Parse("2025-11-24"),
                        new List<CommentDto>(), 1, new Team("Team1"));
            var issue2 = new IssueDto(2, "ISSUE-2", "Title2", "Desc2", IssueStatus.NEW,
                        IssuePriority.HIGH, 1, 2, DateTime.Parse("2025-11-25"), DateTime.Parse("2025-11-26"), DateTime.Parse("2025-11-27"),
                        new List<CommentDto>(), 1, new Team("Team2"));
            return new List<IssueDto> { issue1, issue2 };
        }
    }
}
