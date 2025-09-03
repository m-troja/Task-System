namespace Task_System.Model
{
    public class User
    {
        internal int id;

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public User(int id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
        }

        public User(int id, string name, string email, string password, ICollection<Role> roles) : this(id, name, email, password)
        {
            Roles = roles;
        }
    }
}
