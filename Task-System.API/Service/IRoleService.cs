using Task_System.Model.Entity;

namespace Task_System.Service
{
    public interface IRoleService
    {
        Task<Role> GetRoleByName(string name);
    }
}

