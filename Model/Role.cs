namespace Task_System.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();

        public Role() { }

        public Role(string name)
        {
            Name = name;
        }
    }
}
