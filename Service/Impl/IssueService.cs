using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Formats.Asn1;
using System.Text.Json;
using Task_System.Data;
using Task_System.Exception.IssueException;
using Task_System.Exception.ProjectException;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Config;

namespace Task_System.Service.Impl
{
    public class IssueService : IIssueService
    {
        private readonly PostgresqlDbContext _db;
        private readonly IUserService _us;
        private readonly CommentCnv _commentCnv;
        private readonly IssueCnv _issueCnv;
        private readonly IProjectService _projectService;
        private readonly ILogger<IssueService> l;

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
            l = logger;
        }

        public async Task<Issue> CreateIssueAsync(CreateIssueRequest cir)
        {
            l.log($"Starting issue creation for authorId: {cir.AssigneeId}, projectId: {cir.ProjectId}");

            var author = await _us.GetByIdAsync(cir.AuthorId);
            User? assignee = cir.AssigneeId.HasValue ? await _us.GetByIdAsync(cir.AssigneeId.Value) : null;

            DateTime? dueDateUtc = null;
            if (!string.IsNullOrEmpty(cir.DueDate))
            {
                dueDateUtc = DateTime.SpecifyKind(DateTime.Parse(cir.DueDate), DateTimeKind.Utc);
                l.log($"Parsed due date UTC: {dueDateUtc}");
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
                l.log("Cannot create issue because project {cir.ProjectId} was not found");
                throw new IssueCreationException("Cannot create issue: " + e.Message);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {



                _db.Issues.Add(issue);
                await _db.SaveChangesAsync();

                l.log($"Issue with ID {issue.Id} created successfully");
                
                var key = new Key(project, issue);
                issue.Key = key;

                _db.Keys.Add(key);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                l.log($"Keystring {issue.Key.KeyString} for {issue.Id}");

                l.LogInformation("Key {KeyString} associated with issue {IssueId}", key.KeyString, issue.Id);
            }
            catch (System.Exception )
            {
                l.log($"Error occurred while creating issue for project {cir.ProjectId}");
                await transaction.RollbackAsync();
                throw;
            }

            return issue;
        }

        public Task<bool> DeleteIssueAsync(int id)
        {
            l.log($"DeleteIssueAsync is not implemented yet for issueId {id}");
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Issue>> GetAllAsync()
        {
            l.log($"GetAllAsync is not implemented yet");
            throw new NotImplementedException();
        }

        public async Task<IssueDto> GetIssueDtoByIdAsync(int id)
        {
            l.log($"Fetching issue DTO for issueId {id}");
            Issue issue = await GetIssueByIdAsync(id);
            l.log($"Fetching done");

            if (issue.Key == null)
            {
                l.log($"issue.key is null :(");
                throw new System.Exception("issue.key is null :(");
            }
            if(issue.Key.KeyString == null)
            {
                l.log("issue.Key.KeyString is null :(");
                throw new System.Exception("issue.Key.KeyString is null :(");
            }
            l.log($"Key is {issue.Key}, keystring {issue.Key.KeyString}");



            var issueDto = _issueCnv.ConvertIssueToIssueDto(issue);
            return issueDto;
        }

        public async Task<Issue> GetIssueByIdAsync(int id)
        {
            l.log($"Fetching issue entity for issueId {id}");
            Issue? issue = await _db.Issues
                                      .Include(i => i.Key)      
                                      .Include(i => i.Project)  
                                      .Include(i => i.Comments)  
                                      .FirstOrDefaultAsync(i => i.Id == id); return issue ?? throw new IssueNotFoundException("Issue " + id + " was not found");
            l.log($"Fetched issue: {issue}");
        }
        public async Task<IssueDto> GetIssueDtoByKeyAsync(string keyString)
        {
            l.log($"Fetching issue DTO for key {keyString}");
            int issueId = await GetIssueIdFromKey(keyString);
            Project? project = await GetProjectFromKey(keyString);

            Issue? issue = await _db.Issues
                .Where(i => i.Id == issueId)
                .Where(i => i.Project.ShortName == project.ShortName)
                .Include(i => i.Key)
                .Include(i => i.Project)
                .Include(i => i.Comments)
                .FirstOrDefaultAsync();
            l.log($"Fetched issue: {issue}");
            if (issue == null) throw new IssueNotFoundException("Issue was not found");
            l.log($"issue.Key {issue.Key}");

            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(issue);
            l.log($"Converted issue to DTO: {issueDto}");
            return issueDto;
        }

        public async Task<IssueDto> AssignIssueAsync(AssignIssueRequest air)
        {
            l.log($"Assigning issue {air.key} to user {air.assigneeId}");
            User assignee = await _us.GetByIdAsync(air.assigneeId);
            l.log($"Fetched assignee: {assignee}");
            int issueId = await GetIssueIdFromKey(air.key);
            l.log($"Resolved issueId {issueId} from key {air.key}");
            Issue issue = await GetIssueByIdAsync(issueId);
            issue.Assignee = assignee;
            issue.AssigneeId = assignee.Id;
            l.log($"Set assignee {issue.Assignee} for issue {issue.Id}");

            Issue updatedIssue = await UpdateIssueAsync(issue);
            l.log($"Updated issue {updatedIssue.Id} in database");

            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(updatedIssue);
            l.log($"Converted updated issue to DTO: {issueDto}");
            return issueDto;
        }

        public async Task<int> GetIssueIdFromKey(string key)
        {
            l.log($"Getting issueId from key {key}");
            int lastDash = key.LastIndexOf('-');
            string shortName = lastDash >= 0 ? key.Substring(0, lastDash) : key; // if no dash, take whole string
            string numberPart = lastDash >= 0 ? key.Substring(lastDash + 1) : "";
            int number = int.TryParse(numberPart, out int n) ? n : 0;
            l.log($"shortName {shortName}, numberPart {numberPart}");
            Issue? issue = await _db.Issues
                .Where(i => i.Id == number)
                .Where(i => i.Project.ShortName == shortName)
                .Include(i => i.Key)
                .Include(i => i.Project)
                .FirstOrDefaultAsync();
            if (issue == null) throw new IssueNotFoundException("Issue was not found");
            l.log($"Found issue with ID {issue.Id} for key {key}");
            return issue.Id;
        }

        public async Task<Project> GetProjectFromKey(string key)
        {
            l.log($"Getting project from key {key}");
            int lastDash = key.LastIndexOf('-');
            string shortName = lastDash >= 0 ? key.Substring(0, lastDash) : key; // if no dash, take whole string
            Project? project = await _db.Projects
                .Where(p => p.ShortName == shortName)
                .FirstOrDefaultAsync();
            if (project == null) throw new ProjectNotFoundException("Project was not found");
            l.log($"Found project with ID {project.Id} for key {key}");
            return project;
        }

        public async Task<Issue> UpdateIssueAsync(Issue issue)
        {
            l.log($"Updating issue {issue.Id}");
            issue.UpdatedAt = DateTime.UtcNow;
            _db.Issues.Update(issue);
            await _db.SaveChangesAsync();
            return issue;
        }
    }
}
