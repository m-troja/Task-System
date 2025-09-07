using Microsoft.AspNetCore.Mvc;
using Task_System.Exception.IssueException;
using Task_System.Model.DTO;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/issue")]

public class IssueController : ControllerBase
{
    private readonly IIssueService _is;

    [HttpPost]
    public async Task<ActionResult<IssueCreatedResponse>> CreateIssue(CreateIssueRequest cir)
    {
        Issue issue;
        try 
        {
            issue = await _is.CreateIssueAsync(cir);
        }
        catch (IssueCreationException)
        {

            throw;
        }

        var response = new IssueCreatedResponse(ResponseType.ISSUE_CREATED_OK, issue.Key.KeyString);
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
