using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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
        private readonly ILogger<IssueService> _logger;

        public IssueService(PostgresqlDbContext db,
                            IUserService us,
                            CommentCnv commentCnv,
                            IssueCnv issueCnv,
                            IProjectService projectService,
                            ILogger<IssueService> logger)
        {
            _db = db;
            _us = us;
            _commentCnv = commentCnv;
            _issueCnv = issueCnv;
            _projectService = projectService;
            _logger = logger;
        }

        public async Task<Issue> CreateIssueAsync(CreateIssueRequest cir)
        {
            _logger.LogInformation("Starting issue creation for authorId: {AuthorId}, projectId: {ProjectId}", cir.AuthorId, cir.ProjectId);

            var author = await _us.GetByIdAsync(cir.AuthorId);
            User? assignee = cir.AssigneeId.HasValue ? await _us.GetByIdAsync(cir.AssigneeId.Value) : null;

            DateTime? dueDateUtc = null;
            if (!string.IsNullOrEmpty(cir.DueDate))
            {
                dueDateUtc = DateTime.SpecifyKind(DateTime.Parse(cir.DueDate), DateTimeKind.Utc);
                _logger.LogDebug("Parsed due date UTC: {DueDateUtc}", dueDateUtc);
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

            Project? project;
            try
            {
                project = await _projectService.GetProjectById(cir.ProjectId);
            }
            catch (ProjectNotFoundException e)
            {
                _logger.LogError(e, "Cannot create issue because project {ProjectId} was not found", cir.ProjectId);
                throw new IssueCreationException("Cannot create issue: " + e.Message);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {



                _db.Issues.Add(issue);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Issue with ID {IssueId} created successfully", issue.Id);
                
                var key = new Key(project, issue);
                issue.Key = key;

                _db.Keys.Add(key);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Keystring {issue.Key.KeyString} for issueId {}", issue.Key.KeyString, issue.Id);

                _logger.LogInformation("Key {KeyString} associated with issue {IssueId}", key.KeyString, issue.Id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating issue for project {ProjectId}", cir.ProjectId);
                await transaction.RollbackAsync();
                throw;
            }

            return issue;
        }

        public Task<bool> DeleteIssueAsync(int id)
        {
            _logger.LogWarning("DeleteIssueAsync is not implemented yet for issueId {IssueId}", id);
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Issue>> GetAllAsync()
        {
            _logger.LogWarning("GetAllAsync is not implemented yet");
            throw new NotImplementedException();
        }

        public async Task<IssueDto> GetIssueDtoByIdAsync(int id)
        {
            _logger.LogInformation("Fetching issue DTO for issueId {IssueId}", id);
            Issue issue = await GetIssueByIdAsync(id);
            _logger.LogDebug("Fetching done");

            if (issue.Key == null)
            {
                _logger.LogDebug("issue.key is null :(");
                throw new System.Exception("issue.key is null :(");
            }
            if(issue.Key.KeyString == null)
            {
                _logger.LogDebug("issue.Key.KeyString is null :(");
                throw new System.Exception("issue.Key.KeyString is null :(");
            }
            _logger.LogDebug("Key is {}, keystring {}", issue.Key, issue.Key.KeyString);



            var issueDto = _issueCnv.ConvertIssueToIssueDto(issue);
            return issueDto;
        }

        public async Task<Issue> GetIssueByIdAsync(int id)
        {
            _logger.LogDebug("Fetching issue entity for issueId {IssueId}", id);
            Issue? issue = await _db.Issues
                                      .Include(i => i.Key)      
                                      .Include(i => i.Project)  
                                      .Include(i => i.Comments)  
                                      .FirstOrDefaultAsync(i => i.Id == id); return issue ?? throw new IssueNotFoundException("Issue " + id + " was not found");
        }

        public async Task<Issue> UpdateIssueAsync(Issue issue)
        {
            _logger.LogWarning("UpdateIssueAsync is not implemented yet for issueId {IssueId}", issue.Id);
            throw new NotImplementedException();
        }

        public async Task<IssueDto> GetIssueDtoByKeyAsync(string keyString)
        {
            int lastDash = keyString.LastIndexOf('-');
            string shortName = lastDash >= 0 ? keyString.Substring(0, lastDash) : keyString; // if no dash, take whole string
            string numberPart = lastDash >= 0  ? keyString.Substring(lastDash + 1)  : "";

            int number = int.TryParse(numberPart, out int n) ? n : 0;
            _logger.LogDebug("shortName {shortName}, numberPart {numberPart}", shortName, numberPart);

            Issue? issue = await _db.Issues
                .Where(i => i.Id == number)
                .Where(i => i.Project.ShortName == shortName)
                .Include(i => i.Key)
                .Include(i => i.Project)
                .Include(i => i.Comments)
                .FirstOrDefaultAsync();
            if (issue == null) throw new IssueNotFoundException("Issue was not found");
            _logger.LogDebug("issue.Key {issue.Key}", issue.Key);

            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(issue);
            return issueDto;
        }
    }
}
