using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO.Cnv;

public class IssueCnv
{
    public IssueDto ConvertIssueToIssueDto(Issue issue, List<CommentDto> commentDtos)
    {
        var issueDto = new IssueDto(
                issue.Id,
                issue.Title,
                issue.Description ?? "null",
                issue.Status,
                issue.AuthorId,
                issue.AssigneeId ?? 0,
                issue.CreatedAt,
                issue.DueDate,
                issue.UpdatedAt,
                commentDtos
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
