using System.Runtime.InteropServices;
using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO.Cnv;

public class IssueCnv
{
    private readonly CommentCnv _commentCnv;
    public IssueDto ConvertIssueToIssueDto(Issue Issue)
    {
        ICollection<CommentDto> commentDtos = _commentCnv.ConvertCommentListToCommentDtoList(Issue.Comments);

        var issueDto = new IssueDto(
            Issue.Id,
                Issue.Key.KeyString,
                Issue.Title,
                Issue.Description ?? "No description",
                Issue.Status,
                Issue.Priority ?? IssuePriority.NORMAL,
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

    public List<IssueDto> ConvertIssueListToIssueDtoList(List<Issue> issues)
    {
        var issueDtos = new List<IssueDto>();
        foreach (var issue in issues)
        {
            issueDtos.Add(ConvertIssueToIssueDto(issue));
        }
        return issueDtos;
    }

    public IssueCnv(CommentCnv commentCnv)
    {
        _commentCnv = commentCnv;
    }
}
