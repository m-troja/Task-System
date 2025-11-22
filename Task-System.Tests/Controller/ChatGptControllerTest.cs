using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_System.Controller;
using Task_System.Data;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Controller;
public class ChatGptControllerTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILoginService> _mockLoginService;
    private readonly Mock<IIssueService> _mockAuthService;

    public ILogger<ChatGptController> GetLogger() =>
        new LoggerFactory().CreateLogger<ChatGptController>();

    private ChatGptController CreateController(
        Mock<IUserService> mu,
        Mock<IIssueService> mi)
    {
        var issuesCnvLogger = new LoggerFactory().CreateLogger<IssueCnv>();
        var commentCnv = new CommentCnv();
        var issueCnv = new IssueCnv(commentCnv, issuesCnvLogger);
        return new ChatGptController(mu.Object, GetLogger(), mi.Object, issueCnv);
    }

    [Fact]
    public async Task GetUserBySlackUserId_ShouldReturnUserDto_WhenUserExists()
    {
        // given
        var mu = new Mock<IUserService>();
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mu, mi);

        var slackUserId = "U12345678";
        var expectedUserDto = BuildUserDto(slackUserId);
        mu.Setup(service => service.GetUserBySlackUserIdAsync(slackUserId))
            .ReturnsAsync(expectedUserDto);
        
        // when
        var result = await controller.GetUserBySlackUserId(slackUserId);

        // then
        Assert.Equal(expectedUserDto, result);
    }

    [Fact]
    public async Task CreateIssueBySlack_ShouldCreateIssue()
    {
        // given
        var mu = new Mock<IUserService>();
        var mi = new Mock<IIssueService>();
        var controller = CreateController(mu, mi);
        var req = BuildSlackCreateIssueRequest();
        var expectedIssueDto = BuildIssueDtoChatGpt(req);
        mi.Setup(service => service.CreateIssueBySlackAsync(req)).ReturnsAsync(expectedIssueDto);

        // when
        var result = await controller.CreateIssueBySlack(req);

        // then
        Assert.Equal(expectedIssueDto, result);
    }

    [Fact]
    public async Task AssignIssueByChatGpt_ShouldReturnAssignedDto()
    {
        // given
        var mu = new Mock<IUserService>();
        var mi = new Mock<IIssueService>();
        var issueCnv = new IssueCnv(new CommentCnv(), new LoggerFactory().CreateLogger<IssueCnv>());
        var controller = CreateController(mu, mi);
        var req = new AssignIssueRequestChatGpt("PROJ-1", "U12345678");
        var expectedIssue = BuildIssue(req);
        var expectedConvertedIssue = issueCnv.ConvertIssueToIssueDtoChatGpt(expectedIssue);

        mi.Setup(service => service.AssignIssueBySlackAsync(req)).ReturnsAsync(expectedIssue);

        // when
        var result = await controller.AssignIssueByChatGpt(req);

        // then
        Assert.Equal(expectedConvertedIssue.Id, result.Id);
        Assert.Equal(expectedConvertedIssue.Key, result.Key);
        Assert.Equal(expectedConvertedIssue.ProjectId, result.ProjectId);
        Assert.Equal(expectedConvertedIssue.AssigneeSlackId, result.AssigneeSlackId);
        Assert.Equal(expectedConvertedIssue.AuthorSlackId, result.AuthorSlackId);
    }

    private UserDto BuildUserDto(string slackUserId)
    {
        return new UserDto(
            1,
            "John",
            "Doe",
            "email@test.com",
            new List<string>() { Role.ROLE_USER },
            new List<string>() { },
            false,
            slackUserId
            );
    }
    private SlackCreateIssueRequest BuildSlackCreateIssueRequest()
    {
        return new SlackCreateIssueRequest(
            "title",
            "description",
            "HIGH",
            "U87654321",
            "U87654322",
            "2025-12-31",
            1 );
    }

    private IssueDtoChatGpt BuildIssueDtoChatGpt(SlackCreateIssueRequest req)
    {
        var parsedPriority = Enum.TryParse<IssuePriority>(req.priority, out var priority)
        ? priority
        : IssuePriority.NORMAL;

        return new IssueDtoChatGpt(
            1,
            "PROJ-1",
            req.title,
            req.description,
            Model.IssueFolder.IssueStatus.NEW,
            parsedPriority,
            req.authorSlackId,
            req.assigneeSlackId,
            DateTime.Parse("2025-01-01"),
            req.dueDate != null ? DateTime.Parse(req.dueDate) : null,
            DateTime.Parse("2025-01-01"),
            new List<CommentDto>(),
            1
            );
    }

    private Issue BuildIssue(AssignIssueRequestChatGpt req)
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
        { Id = 1 ,
        CreatedAt = DateTime.Parse("2025-01-02"),
        UpdatedAt = DateTime.Parse("2025-01-03"),
        Comments = new List<Comment>() { } ,
        Key = key
        };
    }

}
