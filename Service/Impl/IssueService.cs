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
using Task_System.Log;

namespace Task_System.Service.Impl
{
    public class IssueService : IIssueService
    {
        private readonly PostgresqlDbContext _db;
        private readonly IUserService _userService;
        private readonly CommentCnv _commentCnv;
        private readonly IssueCnv _issueCnv;
        private readonly IProjectService _projectService;
        private readonly ILogger<IssueService> l;
        private readonly ITeamService _teamService;
        private IActivityService _activityService;

        public IssueService(PostgresqlDbContext db, IUserService userService, CommentCnv commentCnv, IssueCnv issueCnv, IProjectService projectService, ILogger<IssueService> l, ITeamService teamService, IActivityService activityService)
        {
            _db = db;
            _userService = userService;
            _commentCnv = commentCnv;
            _issueCnv = issueCnv;
            _projectService = projectService;
            this.l = l;
            _teamService = teamService;
            _activityService = activityService;
        }

        public async Task<Issue> CreateIssueAsync(CreateIssueRequest cir)
        {
            l.log($"Starting issue creation for authorId: {cir.AssigneeId}, projectId: {cir.ProjectId}");

            var author = await _userService.GetByIdAsync(cir.AuthorId);
            User? assignee = cir.AssigneeId.HasValue ? await _userService.GetByIdAsync(cir.AssigneeId.Value) : null;

            DateTime? dueDateUtc = null;
            if (!string.IsNullOrEmpty(cir.DueDate))
            {
                dueDateUtc = DateTime.SpecifyKind(DateTime.Parse(cir.DueDate), DateTimeKind.Utc);
                l.log($"Parsed due date UTC: {dueDateUtc}");
            }


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

            Issue issue;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                int maxIdInsideProject = await _db.Issues
                    .Where(i => i.ProjectId == cir.ProjectId)
                    .MaxAsync(i => (int?)i.IdInsideProject) ?? 0;

                int nextIdInsideProject = maxIdInsideProject + 1;

                issue = new Issue
                {
                    Title = cir.Title,
                    Description = cir.Description,
                    Priority = !string.IsNullOrEmpty(cir.Priority) ? Enum.Parse<IssuePriority>(cir.Priority) : null,
                    Author = author,
                    AuthorId = author.Id,
                    Assignee = assignee,
                    AssigneeId = assignee?.Id,
                    DueDate = dueDateUtc,
                    ProjectId = cir.ProjectId,
                    IdInsideProject = nextIdInsideProject
                };


                _db.Issues.Add(issue);
                await _db.SaveChangesAsync();

                l.log($"Issue of ID {issue.Id} created successfully");
                
                var key = new Key(project, issue);
                issue.Key = key;

                _db.Keys.Add(key);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                
                l.log($"Keystring {issue.Key.KeyString} for id {issue.Id} and IdInsideProject {issue.IdInsideProject}");
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
                                      .FirstOrDefaultAsync(i => i.Id == id); 
            l.log($"Fetched issue: {issue}");

            return issue ?? throw new IssueNotFoundException("Issue " + id + " was not found");
        }
        public async Task<IssueDto> GetIssueDtoByKeyAsync(string keyString)
        {
            l.log($"Fetching issue DTO for key {keyString}");
            int issueId = GetIssueIdInsideProjectFromKey(keyString);
            Project? project = await GetProjectFromKey(keyString);

            Issue? issue = await _db.Issues
                .Where(i => i.Id == issueId)
                .Where(i => i.Project != null && i.Project.ShortName == project.ShortName)
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
            l.log($"Assigning issue {air.IssueId} to user {air.AssigneeId}");
            User newAssignee = await _userService.GetByIdAsync(air.AssigneeId);
            l.log($"Fetched new assignee: {newAssignee}");
            Issue issue = await GetIssueByIdAsync(air.IssueId);
            User? oldAssignee = null;
            if (issue.AssigneeId.HasValue)
            {
                oldAssignee = await _userService.GetByIdAsync(issue.AssigneeId.Value);
            }
            issue.Assignee = newAssignee;
            issue.AssigneeId = newAssignee.Id;
            l.log($"Set assignee {issue.Assignee} for issue {issue.Id}");

            Issue updatedIssue = await UpdateIssueAsync(issue);
            l.log($"Updated issue {updatedIssue.Id} in database");

            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(updatedIssue);
            l.log($"Converted updated issue to DTO: {issueDto}");

            await _activityService.NewAssignIssueActivity(oldAssignee, issue);
            return issueDto;
        }

        public int GetIssueIdInsideProjectFromKey(string key)
        {
            l.log($"Getting issueId from key {key}");
            int lastDash = key.LastIndexOf('-');
            string shortName = lastDash >= 0 ? key.Substring(0, lastDash) : key; // if no dash, take whole string
            string numberPart = lastDash >= 0 ? key.Substring(lastDash + 1) : "";
            int number = int.TryParse(numberPart, out int n) ? n : 0;
            l.log($"shortName {shortName}, numberPart {numberPart}");

            return number;
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

        public async Task<IssueDto> RenameIssueAsync(RenameIssueRequest rir)
        {
            l.log($"Renaming issue {rir.id} to new title: {rir.newTitle}");
            Issue issue = await GetIssueByIdAsync(rir.id);
            issue.Title = rir.newTitle;
            Issue updatedIssue = await UpdateIssueAsync(issue);
            l.log($"Renamed issue {updatedIssue.Id} successfully");
            _db.Issues.Update(updatedIssue);
            await _db.SaveChangesAsync();
            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(updatedIssue);
            return issueDto;
        }

        public async Task<IssueDto> ChangeIssueStatusAsync(ChangeIssueStatusRequest req)
        {
            l.log($"Changing status of issue {req.IssueId} to {req.NewStatus}");

            if (req.NewStatus == null) throw new ArgumentException("NewStatus cannot be null");
            if (req.IssueId <= 0) throw new ArgumentException("IssueId must be positive");
            if (!Enum.TryParse<IssueStatus>(req.NewStatus, true, out var newStatus)  || !Enum.IsDefined(typeof(IssueStatus), newStatus))
            {
                throw new ArgumentException($"Invalid issue status: {req.NewStatus}");
            }

            Issue issue = await GetIssueByIdAsync(req.IssueId);
            issue.Status = Enum.Parse<IssueStatus>(req.NewStatus);

            var UpdatedIssue = await UpdateIssueAsync(issue);
            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(UpdatedIssue);
            return issueDto;
        }

        public async Task<IssueDto> ChangeIssuePriorityAsync(ChangeIssuePriorityRequest req)
        {
            l.log($"Changing priority of issue {req.IssueId} to {req.NewPriority}");
            if (req.NewPriority == null) throw new ArgumentException("NewPriority cannot be null");
            if (req.IssueId <= 0) throw new ArgumentException("IssueId must be positive");
            if (!Enum.TryParse<IssuePriority>(req.NewPriority, true, out var newPriority) || !Enum.IsDefined(typeof(IssuePriority), newPriority))
            {
                throw new ArgumentException($"Invalid issue priority: {req.NewPriority}");
            }
            Issue issue = await GetIssueByIdAsync(req.IssueId);
            issue.Priority = Enum.Parse<IssuePriority>(req.NewPriority);
            var UpdatedIssue = await UpdateIssueAsync(issue);
            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(UpdatedIssue);
            return issueDto;
        }

        public async Task<IssueDto> AssignTeamAsync(AssignTeamRequest req)
        {
            l.log("Assigning team {req.TeamId} to issue {req.IssueId}");
            Team team = await _teamService.GetTeamByIdAsync(req.TeamId);
            Issue issue = await GetIssueByIdAsync(req.IssueId);
            issue.Team = team;
            var UpdatedIssue = await UpdateIssueAsync(issue);
            IssueDto issueDto = _issueCnv.ConvertIssueToIssueDto(UpdatedIssue);
            l.log($"Assigned team {team.Name} to issue {issue.Id} successfully");
            return issueDto;
        }

        public async Task<IEnumerable<IssueDto>> GetAllIssuesByUserId(int userId)
        {
            l.log($"Getting all issues for userId {userId}");
            User user = await _userService.GetByIdAsync(userId);
            List<Issue> issues = await _db.Issues.Where(i => i.AssigneeId == userId || i.AuthorId == userId)
                .Include(i => i.Key)
                .Include(i => i.Project)
                .Include(i => i.Comments)
                .ToListAsync();
            l.log($"Fetched {issues.Count} issues for userId {userId}");
            var issuesDto = _issueCnv.ConvertIssueListToIssueDtoList(issues);
            return issuesDto;
        }

        public async Task<IEnumerable<IssueDto>> GetAllIssuesByProjectId(int projectId)
        {
            l.log($"Getting all issues for projectId {projectId}");
            Project project = await _projectService.GetProjectById(projectId);
            List<Issue> issues = await _db.Issues.Where(i => i.ProjectId == projectId )
                .Include(i => i.Key)
                .Include(i => i.Project)
                .Include(i => i.Comments)
                .ToListAsync();
            l.log($"Fetched {issues.Count} issues for projectId {projectId}");
            var issuesDto = _issueCnv.ConvertIssueListToIssueDtoList(issues);
            return issuesDto;
        }

        public async Task<int> GetIssueIdFromKey(string key)
        {
            int IssueIdInProject = GetIssueIdInsideProjectFromKey(key);
            if (IssueIdInProject == 0) throw new IssueNotFoundException("Issue was not found");
            Project project = await GetProjectFromKey(key);
            Issue issue = await _db.Issues
                .Where( i => i.ProjectId == project.Id && i.IdInsideProject == IssueIdInProject)
                .FirstOrDefaultAsync();
            if (issue == null) throw new IssueNotFoundException("Issue was not found");
            return issue.Id;
        }
    }
}
