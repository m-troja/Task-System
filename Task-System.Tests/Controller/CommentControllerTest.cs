using Microsoft.AspNetCore.Mvc;
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
using Task_System.Model.Request;
using Task_System.Service;
using Xunit;

namespace Task_System.Tests.Controller;
public class CommentControllerTest
{
    private readonly Mock<ICommentService> mc;

    public ILogger<CommentController> GetLogger() =>
        new LoggerFactory().CreateLogger<CommentController>();

    private CommentController CreateController(
        Mock<ICommentService> mc)
    {
        mc = new Mock<ICommentService>();
        return new CommentController(mc.Object, GetLogger());
    }

    [Fact]
    public async Task GetCommentsByIssueId_ShouldReturnComments_WhenCommentsExist()
    {
        // given
        var mc = new Mock<ICommentService>();
        var controller = new CommentController(mc.Object, GetLogger());

        var issueId = 1;

        var expectedComments = new List<CommentDto>
        {
            new CommentDto(1, 1, "Content1", 1, DateTime.Parse("2025-11-01"),  DateTime.Parse("2025-11-01")),
            new CommentDto(2, 1, "Content2", 1, DateTime.Parse("2025-11-02"), DateTime.Parse("2025-11-01"))
        };

        mc.Setup(s => s.GetCommentsByIssueIdAsync(issueId))
          .ReturnsAsync(expectedComments);

        // when
        var result = await controller.GetCommentsByIssueId(issueId);

        // then
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnComments = Assert.IsType<List<CommentDto>>(ok.Value);

        Assert.Equal(expectedComments.Count, returnComments.Count);
        Assert.Equal(expectedComments, returnComments);
    }

    [Fact]
    public async Task DeleteCommentById_ShouldDeleteComment()
    {
        // given
        var mc = new Mock<ICommentService>();
        var controller = new CommentController(mc.Object, GetLogger());
        var commentId = 1;
        mc.Setup(s => s.DeleteCommentById(commentId))
          .Returns(Task.CompletedTask)
          .Verifiable();
        
        // when
        var result = await controller.DeleteCommentById(commentId);
        
        // then
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var message = Assert.IsType<string>(ok.Value);
        Assert.Equal($"Deleted comment by id={commentId}", message);
        mc.Verify(s => s.DeleteCommentById(commentId), Times.Once);
    }

    [Fact]
    public async Task DeleteAllCommentsByIssueId_ShouldDeleteComments()
    {
        var mc = new Mock<ICommentService>();
        var controller = new CommentController(mc.Object, GetLogger());
        var issueId = 1;
        mc.Setup(s => s.DeleteAllCommentsByIssueId(issueId))
          .Returns(Task.CompletedTask)
          .Verifiable();
        // when
        var result = await controller.DeleteAllCommentsByIssueId(issueId);
        // then
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var message = Assert.IsType<string>(ok.Value);
        Assert.Equal($"Deleted all comments for issue id={issueId}", message);
        mc.Verify(s => s.DeleteAllCommentsByIssueId(issueId), Times.Once);
    }

    [Fact]
    public async Task CreateComment_ShouldCreateComment()
    {
        // given
        var mc = new Mock<ICommentService>();
        var controller = new CommentController(mc.Object, GetLogger());
        var createRequest = new CreateCommentRequest("Content", 1, 1);
        var expectedDto = new CommentDto(1, 1, "Content1", 1, DateTime.Parse("2025-11-01"),  DateTime.Parse("2025-11-01"));

        mc.Setup(s => s.CreateCommentAsync(createRequest))
          .ReturnsAsync(expectedDto)
          .Verifiable();
        
        // when
        var result = await controller.CreateComment(createRequest);
        
        // then
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        mc.Verify(s => s.CreateCommentAsync(createRequest), Times.Once);
    }
}
