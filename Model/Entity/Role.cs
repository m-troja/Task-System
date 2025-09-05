namespace Task_System.Model.Entity
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();

        protected Role() { }

        public Role(string name)
        {
            Name = name;
        }
    }
}
