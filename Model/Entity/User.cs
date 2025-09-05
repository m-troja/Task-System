namespace Task_System.Model.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public User(string name, string email, string password, ICollection<Role> roles) : this(name, email, password)
        {
            Roles = roles;
        }

        protected User()
        {
        }
    }
}
