﻿using Task_System.Exception.ProjectException;
using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO.Cnv;

public class ProjectCnv
{
    private readonly IssueCnv _issueCnv;
    public ProjectDto ConvertProjectToProjectDto(Project project)
    {
        if (project == null) throw new ProjectNotFoundException("Project not found");
        if (project.Issues == null) project.Issues = new List<Issue>();
        if (project.Description == null) project.Description = "";

        return new ProjectDto
        {
            Id = project.Id,
            ShortName = project.ShortName,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            Issues = project.Issues?.Select(i => _issueCnv.ConvertIssueToIssueDto(i)).ToList() ?? new List<IssueDto>(),
        };
    }

    public ProjectCnv(IssueCnv issueCnv)
    {
        _issueCnv = issueCnv;
    }
}
