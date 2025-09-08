using Task_System.Model.IssueFolder;

namespace Task_System.Model.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public byte[] Salt { get; set; }   
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
        public ICollection<Issue> AuthoredIssues { get; set; } = new List<Issue>();

        public User(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public User(string firstName, string lastName, string email, string password, ICollection<Role> roles) : this(firstName, lastName, email, password)
        {
            Roles = roles;
        }

        protected User()
        {
        }
    }
}
