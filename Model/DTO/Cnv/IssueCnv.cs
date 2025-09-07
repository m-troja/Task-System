using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO.Cnv;

public class IssueCnv
{
    public IssueDto ConvertIssueToIssueDto(Issue Issue, List<CommentDto> commentDtos)
    {
        var issueDto = new IssueDto(
                Issue.Id,
                Issue.Title,
                Issue.Description ?? "null",
                Issue.Status,
                Issue.AuthorId,
                Issue.AssigneeId ?? 0,
                Issue.CreatedAt,
                Issue.DueDate,
                Issue.UpdatedAt,
                commentDtos,
                Issue.ProjectId
            );
        
        return issueDto;
    }

    public List<IssueDto> ConvertIssueListToIssueDtoList(List<Issue> issues, List<CommentDto> commentDtos)
    {
        var issueDtos = new List<IssueDto>();
        foreach (var issue in issues)
        {
            issueDtos.Add(ConvertIssueToIssueDto(issue, commentDtos));
        }
        return issueDtos;
    }
}
