using Microsoft.AspNetCore.Mvc;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Model.IssueFolder;
using Task_System.Service;
using Task_System.Model.DTO;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/issue")]

public class IssueController : ControllerBase
{
    private readonly IIssueService _is;

    [HttpPost]
    public async Task<ActionResult<IssueCreatedResponse>> CreateIssue(CreateIssueRequest cir)
    {
        var issue = await _is.CreateIssueAsync(cir);
        var response = new IssueCreatedResponse(ResponseType.ISSUE_CREATED_OK, issue.Id);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IssueDto>> GetIssueById(int id)
    {
        var IssueDto = await _is.GetIssueDtoByIdAsync(id);
       
        return Ok(IssueDto);
    }

    public IssueController(IIssueService issueService)
    {
        _is = issueService;
    }
}
