using Microsoft.AspNetCore.Mvc;
using Sprache;
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
    public async Task<ActionResult<Response>> CreateComment([FromBody] CreateCommentRequest cmr)
    {
        await _cs.CreateCommentAsync(cmr);
        Response response = new Response(ResponseType.COMMENT_CREATED_OK, cmr.Content);
        return Ok(response);
    }

    public CommentController(ICommentService cs)
    {
        _cs = cs;
    }
}
