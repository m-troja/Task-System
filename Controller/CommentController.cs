using Microsoft.AspNetCore.Mvc;
using Sprache;
using Task_System.Model.DTO;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _cs;

    [HttpPost]
    [Route("create")]   
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentRequest cmr)
    {
        await _cs.CreateCommentAsync(cmr);
        return Ok();
    }

    public CommentController(ICommentService cs)
    {
        _cs = cs;
    }
}
