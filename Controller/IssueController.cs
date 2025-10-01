using Microsoft.AspNetCore.Mvc;
using Task_System.Exception.IssueException;
using Task_System.Model.DTO;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Response;
using Task_System.Service;
using Task_System.Config;
using Microsoft.AspNetCore.Authorization;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/issue")]

public class IssueController : ControllerBase
{
    private readonly IIssueService _is;
    private readonly ILogger<IssueController> l;

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<IssueCreatedResponse>> CreateIssue(CreateIssueRequest req)
    {
        l.log($"Received create issue request: {req}");
        Issue issue;
        try 
        {
            issue = await _is.CreateIssueAsync(req);
        }
        catch (IssueCreationException)
        {

            throw;
        }

        var response = new IssueCreatedResponse(ResponseType.ISSUE_CREATED_OK, issue.Key.KeyString);
        return Ok(response);
    }

    [Authorize]
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
    public async Task<ActionResult<IssueDto>> AssignIssue([FromBody] AssignIssueRequest req)
    {
        l.log($"Received assign issue request: {req}");
        IssueDto issueDto = await _is.AssignIssueAsync(req);
        return Ok(issueDto);
    }

    [HttpPut("rename")]
    public async Task<ActionResult<IssueDto>> RenameIssue([FromBody] RenameIssueRequest req)
    {
        l.log($"Received rename issue request: {req.id}, {req.newTitle}");
        IssueDto issueDto = await _is.RenameIssueAsync(req);
        return Ok(issueDto);
    }

    [HttpPut("assign-team")]
    public async Task<ActionResult<IssueDto>> AssignTeam([FromBody] AssignTeamRequest req)
    {
        l.log($"Received AssignTeam request: {req.IssueId}, {req.TeamId}");
        IssueDto issueDto = await _is.AssignTeamAsync(req);
        return Ok(issueDto);
    }

    [HttpPut("change-status")]
    public async Task<ActionResult<IssueDto>> ChangeIssueStatus([FromBody] ChangeIssueStatusRequest req)
    {
        l.log($"Received change issue status request: {req.IssueId}, {req.NewStatus}");
        IssueDto issueDto = await _is.ChangeIssueStatusAsync(req);
        return Ok(issueDto);
    }

    [HttpPut("change-priority")]
    public async Task<ActionResult<IssueDto>> ChangeIssuePriority([FromBody] ChangeIssuePriorityRequest req)
    {
        l.log($"Received change issue priority request: {req.IssueId}, {req.NewPriority}");
        IssueDto issueDto = await _is.ChangeIssuePriorityAsync(req);
        return Ok(issueDto);
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<List<IssueDto>>> GetAllIssuesByUserId(int userId)
    {
        l.log($"Received get all issues by user id request: {userId}");
        var issuesDto = await _is.GetAllIssuesByUserId(userId);
        return Ok(issuesDto);
    }


    public IssueController(IIssueService @is, ILogger<IssueController> l)
    {
        _is = @is;
        this.l = l;
    }
}
