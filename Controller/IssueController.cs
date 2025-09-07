using Microsoft.AspNetCore.Mvc;
using Task_System.Exception.IssueException;
using Task_System.Model.DTO;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Task_System.Config;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/issue")]

public class IssueController : ControllerBase
{
    private readonly IIssueService _is;
    private readonly ILogger<IssueController> l;

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<IssueCreatedResponse>> CreateIssue(CreateIssueRequest cir)
    {
        l.log($"Received create issue request: {cir}");
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

    [HttpGet("key/{key}")]
    public async Task<ActionResult<IssueDto>> GetIssueByKey(string key)
    {
        l.log($"Received get issue by key request: {key}");
        var IssueDto = await _is.GetIssueDtoByKeyAsync(key);

        return Ok(IssueDto);

    }
    [HttpGet("id/{id}")]
    public async Task<ActionResult<IssueDto>> GetIssueById(int id)
    {
        l.log($"Received get issue by id request: {id}");
        var IssueDto = await _is.GetIssueDtoByIdAsync(id);
       
        return Ok(IssueDto);
    }

    [HttpPut("assign")]
    public async Task<ActionResult<IssueDto>> AssignIssue([FromBody] AssignIssueRequest air)
    {
        l.log($"Received assign issue request: {air}");
        IssueDto issueDto = await _is.AssignIssueAsync(air);
        return Ok(issueDto);
    }

    public IssueController(IIssueService @is, ILogger<IssueController> l)
    {
        _is = @is;
        this.l = l;
    }
}
