using Microsoft.AspNetCore.Http.HttpResults;
using Task_System.Data;
using Task_System.Exception.IssueException;
using Task_System.Exception.UserException;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service.Impl;

public class CommentService : ICommentService
{
    private readonly PostgresqlDbContext _db;
    private readonly IIssueService _issueService;
    private readonly IUserService _userService;
    private readonly CommentCnv _commentCnv;
    public async Task<CommentDto> CreateCommentAsync(CreateCommentRequest ccr)
    {
        Issue issue = _issueService.GetIssueByIdAsync(ccr.IssueId).Result;
        User user = _userService.GetByIdAsync(ccr.AuthorId).Result;
        Comment comment = new Comment(ccr.Content, user, issue);
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return _commentCnv.ConvertCommentToCommentDto(comment);
    }

    public Task<IEnumerable<Comment>> GetCommentsByIssueIdAsync(int issueId)
    {
        throw new NotImplementedException();
    }

    public CommentService(PostgresqlDbContext db, IIssueService issueService, IUserService userService, CommentCnv commentCnv)
    {
        _db = db;
        _issueService = issueService;
        _userService = userService;
        _commentCnv = commentCnv;
    }
}
