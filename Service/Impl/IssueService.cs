using Task_System.Data;
using Task_System.Model.IssueFolder;
using Task_System.Model.Request;
using Task_System.Model.Entity;

namespace Task_System.Service.Impl
{
    public class IssueService : IIssueService
    {

        private readonly PostgresqlDbContext _db;
        private readonly IUserService _us;
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
				Author = author,
				AuthorId = author.Id,
				Assignee = assignee,
				AssigneeId = assignee?.Id,
				DueDate = dueDateUtc
			};

			_db.Issues.Add(issue);
			await _db.SaveChangesAsync();

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

        public Task<Issue> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Issue> UpdateIssueAsync(Issue issue)
        {
            throw new NotImplementedException();
        }

        public IssueService(PostgresqlDbContext db, IUserService us)
        {
            _db = db;
            _us = us;
        }
    }
}
