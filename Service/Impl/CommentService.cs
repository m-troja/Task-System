using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
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
    private readonly ILogger<CommentService> logger;
    public async Task<CommentDto> CreateCommentAsync(CreateCommentRequest ccr)
    {
        Issue issue = _issueService.GetIssueByIdAsync(ccr.IssueId).Result;
        User user = _userService.GetByIdAsync(ccr.AuthorId).Result;
        Comment comment = new Comment(ccr.Content, user, issue);
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return _commentCnv.ConvertCommentToCommentDto(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByIssueIdAsync(int issueId)
    {
        List<Comment> comments = await _db.Comments.Where(c => c.IssueId == issueId).ToListAsync();

        return _commentCnv.ConvertCommentListToCommentDtoList(comments);
    }

    public CommentService(PostgresqlDbContext db, IIssueService issueService, IUserService userService, CommentCnv commentCnv, ILogger<CommentService> logger)
    {
        _db = db;
        _issueService = issueService;
        _userService = userService;
        _commentCnv = commentCnv;
        this.logger = logger;
    }

    public async Task DeleteAllCommentsByIssueId(int issueId)
    {
        await _db.Database.ExecuteSqlAsync($"DELETE FROM Comments where issue_id={issueId}");
        logger.LogInformation($"Deleted comment where Id={issueId}");
    }
    public async Task DeleteCommentById(int id)
    {
        await _db.Database.ExecuteSqlAsync($"DELETE FROM Comments where id={id}");
        logger.LogInformation($"Deleted comment where Id={id}");
    }

}
