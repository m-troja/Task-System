using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_System.Controller;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Xunit;

namespace Task_System.Tests.Controller;
public class IssueControllerTest
{
    private readonly Mock<IIssueService> mi;
    public ILogger<IssueController> GetLogger() =>
        new LoggerFactory().CreateLogger<IssueController>();

    private IssueController CreateController(
        Mock<IIssueService> mi)
    {
        ILogger<IssueCnv> mockIssueService = new LoggerFactory().CreateLogger<IssueCnv>();
        var commentCnv = new CommentCnv();
        var issueCnv = new IssueCnv(commentCnv, mockIssueService);
        return new IssueController(mi.Object, GetLogger(), issueCnv);
    }

    [Fact]
    public async Task GetAllIssues_ShouldReturnIssues_WhenIssuesExist()
    {
        // given
        var mi = new Mock<IIssueService>();
        var controller = new IssueController(mi.Object, GetLogger(), 
            new IssueCnv(new CommentCnv(), new LoggerFactory().CreateLogger<IssueCnv>()));
        var expectedIssues = new List<Model.DTO.IssueDto>
        {
            new IssueDto(1, "ISSUE-1", "Title1", "Description1", Model.IssueFolder.IssueStatus.NEW, Model.IssueFolder.IssuePriority.HIGH, 1, 2, DateTime.Parse("2025-11-01"), DateTime.Parse("2025-12-01"), DateTime.Parse("2025-11-02"), new List<Model.DTO.CommentDto>(), 1, new Team("Team1")),
            new IssueDto(2, "ISSUE-2", "Title2", "Description2", Model.IssueFolder.IssueStatus.DONE, Model.IssueFolder.IssuePriority.LOW, 2, 3, DateTime.Parse("2025-11-03"), DateTime.Parse("2025-12-03"), DateTime.Parse("2025-11-04"), new List<Model.DTO.CommentDto>(), 1, new Team("Team2"))
        };
        mi.Setup(s => s.GetAllIssues())
          .ReturnsAsync(expectedIssues);
        // when
        var result = await controller.GetAllIssues();
        // then
        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnIssues = Assert.IsType<List<Model.DTO.IssueDto>>(ok.Value);
        Assert.Equal(expectedIssues.Count, returnIssues.Count);
        Assert.Equal(expectedIssues, returnIssues);
    }

    [Fact]
    public async Task GetIssueById_ShouldReturnIssue_WhenExists()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var issue = BuildIssueDto();

        mi.Setup(s => s.GetIssueDtoByIdAsync(1)).ReturnsAsync(issue);

        var result = await controller.GetIssueById(1);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnIssue = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal(issue, returnIssue);
    }

    [Fact]
    public async Task GetIssueByKey_ShouldReturnIssue_WhenExists()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var issue = BuildIssueDto();

        mi.Setup(s => s.GetIssueDtoByKeyAsync("ISSUE-1")).ReturnsAsync(issue);

        var result = await controller.GetIssueByKey("ISSUE-1");

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal(issue, returned);
    }

    [Fact]
    public async Task CreateIssue_ShouldReturnCreatedResponse()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new CreateIssueRequest("Hello", "desc", "NORMAL", 10, 10, null, 100);
        var project = new Project("PROJ", "Desc" );
        var issue = BuildIssue();

        mi.Setup(s => s.CreateIssueAsync(req))
            .ReturnsAsync(issue);

        var result = await controller.CreateIssue(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueCreatedResponse>(ok.Value);

        Assert.Equal("PROJ-1", returned.key);
        Assert.Equal(ResponseType.ISSUE_CREATED_OK, returned.responseType);
    }

    [Fact]
    public async Task AssignIssue_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new AssignIssueRequest(1, 20);

        var issueDto = new IssueDto(1, "ISSUE-1", "T", "D", IssueStatus.NEW,
            IssuePriority.HIGH, 1, 2, DateTime.Now, DateTime.Now, DateTime.Now, new List<CommentDto>(), 1, new Team("Team1"));

        mi.Setup(s => s.AssignIssueAsync(req)).ReturnsAsync(BuildIssue());

        mi.Setup(s => s.GetIssueDtoByIdAsync(1)).ReturnsAsync(issueDto);

        var result = await controller.AssignIssue(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);
    }

    [Fact]
    public async Task RenameIssue_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new RenameIssueRequest(1, "NewTitle");

        var issueDto = new IssueDto(1, "ISSUE-1", "NewTitle", "D", IssueStatus.NEW,
            IssuePriority.HIGH, 1, 2, DateTime.Now, DateTime.Now, DateTime.Now, new List<CommentDto>(), 1, new Team("Team"));

        mi.Setup(s => s.RenameIssueAsync(req)).ReturnsAsync(issueDto);

        var result = await controller.RenameIssue(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal("NewTitle", returned.Title);
    }

    [Fact]
    public async Task AssignTeam_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new AssignTeamRequest(1, 50);

        var issueDto = BuildIssueDto();

        mi.Setup(s => s.AssignTeamAsync(req)).ReturnsAsync(issueDto);

        var result = await controller.AssignTeam(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal("Team", returned.Team.Name);
    }

    [Fact]
    public async Task ChangeIssueStatus_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new ChangeIssueStatusRequest(1, "IN_PROGRESS");

        var issueDto = new IssueDto(1, "ISSUE-1", "T", "D", IssueStatus.DONE,
            IssuePriority.HIGH, 1, 2, DateTime.Now, DateTime.Now, DateTime.Now, new List<CommentDto>(), 1, new Team("Team"));

        mi.Setup(s => s.ChangeIssueStatusAsync(req)).ReturnsAsync(issueDto);

        var result = await controller.ChangeIssueStatus(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal(IssueStatus.DONE, returned.Status);
    }

    [Fact]
    public async Task UpdateIssuePriority_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new ChangeIssuePriorityRequest(1, "HIGH");

        var issueDto = new IssueDto(1, "ISSUE-1", "T", "D", IssueStatus.NEW,
            IssuePriority.LOW, 1, 2, DateTime.Now, DateTime.Now, DateTime.Now, new List<CommentDto>(), 1, new Team("Team"));

        mi.Setup(s => s.ChangeIssuePriorityAsync(req)).ReturnsAsync(issueDto);

        var result = await controller.UpdateIssuePriority(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal(IssuePriority.LOW, returned.Priority);
    }

    [Fact]
    public async Task UpdateDueDate_ShouldReturnUpdatedIssue()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var req = new UpdateDueDateRequest(1, DateTime.Parse("2030-01-01"));

        var issueDto = new IssueDto(1, "ISSUE-1", "T", "D", IssueStatus.NEW,
            IssuePriority.HIGH, 1, 2, DateTime.Now, DateTime.Parse("2030-01-01"), DateTime.Now, new List<CommentDto>(), 1, new Team("Team"));

        mi.Setup(s => s.UpdateDueDateAsync(req)).ReturnsAsync(issueDto);

        var result = await controller.UpdateDueDate(req);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returned = Assert.IsType<IssueDto>(ok.Value);

        Assert.Equal(DateTime.Parse("2030-01-01"), returned.DueDate);
    }

    [Fact]
    public async Task GetAllIssuesByUserId_ShouldReturnIssues()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var issues = new List<IssueDto>();

        mi.Setup(s => s.GetAllIssuesByUserId(1)).ReturnsAsync(issues);

        var result = await controller.GetAllIssuesByUserId(1);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        Assert.Equal(issues, ok.Value);
    }

    [Fact]
    public async Task GetAllIssuesByProjectId_ShouldReturnIssues()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        var issues = new List<IssueDto>();

        mi.Setup(s => s.GetAllIssuesByProjectId(1)).ReturnsAsync(issues);

        var result = await controller.GetAllIssuesByProjectId(1);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        Assert.Equal(issues, ok.Value);
    }

    [Fact]
    public async Task DeleteIssueById_ShouldReturnConfirmation()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        mi.Setup(s => s.deleteIssueById(1)).Returns(Task.CompletedTask);

        var result = await controller.DeletelIssueById(1);

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);

        Assert.Equal("Deleted issue 1", ok.Value);
    }

    [Fact]
    public async Task DeleteAllIssues_ShouldReturnConfirmation()
    {
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mi);

        mi.Setup(s => s.deleteAllIssues()).Returns(Task.CompletedTask);

        var result = await controller.DeleteAllIssues();

        var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);

        Assert.Equal("All issues deleted successfully", ok.Value);
    }

    private Issue BuildIssue()
    {
        var project = new Project() { Id = 1, ShortName = "PROJ" };
        var key = new Key() { Id = 1, KeyString = "PROJ-1", Project = project, ProjectId = project.Id };

        return new Issue(
            "Title",
            "Desc",
            IssuePriority.HIGH,
            new User { Id = 1, FirstName = "John", LastName = "Doe" },
            new User { Id = 2, FirstName = "John", LastName = "Doe" },
            DateTime.Parse("2025-01-01"),
            1,
            2,
            1,
            2
            )
        {
            Id = 1,
            CreatedAt = DateTime.Parse("2025-01-02"),
            UpdatedAt = DateTime.Parse("2025-01-03"),
            Comments = new List<Comment>() { },
            Key = key
        };
    }

    private User BuildUser()
    {
        return new User
        (
            "John",
            "Doe",
            "test@test.com",
            "password",
            new byte[] { Byte.Parse("111") },
            new Role(Role.ROLE_USER)
        );
    }
    
    private IssueDto BuildIssueDto()
    {
        return new IssueDto(1, "ISSUE-1", "Title", "Desc", IssueStatus.NEW,
                    IssuePriority.HIGH, 1, 2, DateTime.Now, DateTime.Now, DateTime.Now, new List<CommentDto>(), 1, new Team("Team"));    
    }
}
