using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using Task_System.Model.DTO;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;

namespace Task_System.Controller;

[Authorize]
[ApiController]
[Route("api/v1/comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _cs;
    private readonly ILogger<CommentController> logger;

    [HttpPost]
    [Route("create")]   
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentRequest cmr)
    {
        await _cs.CreateCommentAsync(cmr);
        return Ok();
    }
    [HttpGet]
    [Route("issue/{issueId:int}")]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsByIssueId(int issueId)
    {
        logger.LogInformation($"ReceivedGetCommentsByIssueId {issueId}");
        var comments = await _cs.GetCommentsByIssueIdAsync(issueId);
        return Ok(comments);
    }

    [HttpDelete]
    [Route("issue/{issueId:int}")]
    public async Task<ActionResult<string>> DeleteAllCommentsByIssueId(int issueId)
    {
        await _cs.DeleteAllCommentsByIssueId(issueId);
        return Ok($"Deleted comment by issueId={issueId}");
    }
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<string>> DeleteCommentById(int id)
    {
        await _cs.DeleteCommentById(id);
        return Ok($"Deleted comment by id={id}");
    }

    public CommentController(ICommentService cs, ILogger<CommentController> logger)
    {
        _cs = cs;
        this.logger = logger;
    }
}
