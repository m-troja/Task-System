using Microsoft.AspNetCore.Http.HttpResults;
using Task_System.Data;
using Task_System.Exception.IssueException;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service.Impl;

public class CommentService : ICommentService
{
    private readonly PostgresqlDbContext _db;
    private readonly IIssueService _issueService;
    private readonly IUserService _userService;
    public async Task<Comment> CreateCommentAsync(CreateCommentRequest ccr)
    {
        Issue issue = _issueService.GetIssueByIdAsync(ccr.IssueId).Result;
        if (issue == null) throw new IssueNotFoundException("Issue " + ccr.IssueId + " was not found");
        User user = _userService.GetByIdAsync(ccr.AuthorId).Result;
        if (user == null) throw new UserNotFoundException("User " + ccr.AuthorId + " was not found");
        Comment comment = new Comment(ccr.Content, user, issue);
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public Task<IEnumerable<Comment>> GetCommentsByIssueIdAsync(int issueId)
    {
        throw new NotImplementedException();
    }

    public CommentService(PostgresqlDbContext db, IIssueService issueService, IUserService userService)
    {
        _db = db;
        _issueService = issueService;
        _userService = userService;
    }
}
