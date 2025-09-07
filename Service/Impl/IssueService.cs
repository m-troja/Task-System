using Microsoft.EntityFrameworkCore;
using Task_System.Data;
using Task_System.Exception.IssueException;
using Task_System.Exception.ProjectException;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service.Impl
{
    public class IssueService : IIssueService
    {

        private readonly PostgresqlDbContext _db;
        private readonly IUserService _us;
        private readonly CommentCnv _commentCnv;
        private readonly IssueCnv _issueCnv;
        private readonly IProjectService _projectService;

        public async Task<Issue> CreateIssueAsync(CreateIssueRequest cir)
		{
			var author = await _us.GetByIdAsync(cir.AuthorId);
			User? assignee = cir.AssigneeId.HasValue ? await _us.GetByIdAsync(cir.AssigneeId.Value) : null;

			//  DueDate from JSON to UTC
			DateTime? dueDateUtc = null;
			if (!string.IsNullOrEmpty(cir.DueDate))
			{
				dueDateUtc = DateTime.SpecifyKind(DateTime.Parse(cir.DueDate), DateTimeKind.Utc);
			}

			var issue = new Issue
			{
				Title = cir.Title,
				Description = cir.Description,
                Priority = !string.IsNullOrEmpty(cir.Priority) ? Enum.Parse<IssuePriority>(cir.Priority) : null,
				Author = author,
				AuthorId = author.Id,
				Assignee = assignee,
				AssigneeId = assignee?.Id,
				DueDate = dueDateUtc,
                ProjectId = cir.ProjectId
            };

            Project? project = null;
            try
            {
                project = await _projectService.GetProjectById(cir.ProjectId);
            }
            catch (ProjectNotFoundException e )
            {
                throw new IssueCreationException("Cannot create issue: " + e.Message);
            }
            
            using var transaction = await _db.Database.BeginTransactionAsync();

            _db.Issues.Add(issue);
            await _db.SaveChangesAsync();

            var key = new Key(project, issue);
            issue.Key = key;
            _db.Keys.Add(key);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();


            return issue;
		}

		public Task<bool> DeleteIssueAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Issue>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IssueDto> GetIssueDtoByIdAsync(int id)
        {
            Issue? issue = await GetIssueByIdAsync(id);
            var IssueDto = _issueCnv.ConvertIssueToIssueDto(issue);
            return IssueDto;
        }
        public async Task<Issue> GetIssueByIdAsync(int id)
        {
            Issue? issue = await _db.Issues.Where(i => i.Id == id).FirstOrDefaultAsync();
            return issue == null ? throw new IssueNotFoundException("Issue " + id + " was not found") : issue;
        }
        public Task<Issue> UpdateIssueAsync(Issue issue)
        {
            throw new NotImplementedException();
        }

        public IssueService(PostgresqlDbContext db, IUserService us, CommentCnv commentCnv, IssueCnv issueCnv, IProjectService projectService)
        {
            _db = db;
            _us = us;
            _commentCnv = commentCnv;
            _issueCnv = issueCnv;
            _projectService = projectService;
        }
    }
}
