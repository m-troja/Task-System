using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Task_System.Log;
using Task_System.Model;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Service;
using Task_System.Service.Impl;

namespace Task_System.Controller
{
    [ApiController]
    [Route("api/v1/team")]
    public class TeamController : ControllerBase
    {
        private readonly IUserService _us;
        private readonly TeamCnv _teamCnv;
        private readonly ILogger<UserController> l;
        private readonly ITeamService _ts;

        public TeamController(IUserService us, TeamCnv teamCnv, ILogger<UserController> logger, ITeamService ts)
        {
            _us = us;
            _teamCnv = teamCnv;
            l = logger;
            _ts = ts;
        }

        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<UserDto>> GetTeamById(int id)
        {
            l.log($"Fetching team by id: {id}");
            var team =  await _ts.GetTeamByIdAsync(id);

            return Ok(_teamCnv.ConvertTeamToTeamDto(team));
        }


        [HttpGet("all")]
        public async Task<ActionResult<List<TeamDto>>> GetAllTeams()
        {
            l.log("Fetching all teams");
            var teams = await _ts.GetAllTeamsAsync();
            return Ok(_teamCnv.ConvertTeamsToTeamDtos(teams));
        }

        [HttpPost("create")]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamRequest req)
        {
            l.log($"Creating team with name: {req.Name}");
            
            var Team = await _ts.AddTeamAsync(req);
            return Ok(_teamCnv.ConvertTeamToTeamDto(Team));
        }

        [HttpGet("issues/{teamId:int}")]
        public async Task<ActionResult<List<IssueDto>>> GetIssuesByTeamId(int teamId)
        {
            l.log($"Fetching issues by teamId {teamId}");
            var issues = await _ts.GetIssuesByTeamId(teamId);
            return Ok(issues);
        }

        [HttpGet("users/{teamId:int}")]
        public async Task<ActionResult<List<UserDto>>> GetUsersByTeamId(int teamId)
        {
            l.log($"Fetching users by teamId {teamId}");
            var issues = await _ts.GetIssuesByTeamId(teamId);
            return Ok(issues);
        }
    }
}
