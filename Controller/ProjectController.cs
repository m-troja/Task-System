using Microsoft.AspNetCore.Mvc;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Request;
using Task_System.Service;

namespace Task_System.Controller;

[ApiController]
[Route("api/v1/project")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _ps;
    private readonly ProjectCnv _projectCnv;

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
    {
        var project = await _ps.GetProjectById(id);
        ProjectDto projectDto = _projectCnv.ConvertProjectToProjectDto(project);
        return Ok(projectDto);
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectRequest cpr)
    {
        var createdProject = await _ps.CreateProject(cpr);
        return Ok(createdProject);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ProjectDto>>> GetAllProjects()
    {
        var projects = await _ps.GetAllProjectsAsync();
        return Ok(projects);
    }

    public ProjectController(IProjectService ps, ProjectCnv projectCnv)
    {
        _ps = ps;
        _projectCnv = projectCnv;
    }
}
