using Task_System.Model.IssueFolder;

namespace Task_System.Model.DTO.Cnv;

public class CommentCnv
{
    public CommentDto ConvertCommentToCommentDto(Comment comment)
    {
        return new CommentDto(comment.Id, comment.IssueId, comment.Content, comment.AuthorId, comment.CreatedAt);
    }

    public List<CommentDto> ConvertCommentListToCommentDtoList(List<Comment> comments)
    {
        List<CommentDto> commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            commentDtos.Add(ConvertCommentToCommentDto(comment));
        }
        return commentDtos;
    }
}
