using Task_System.Model.Entity;

namespace Task_System.Service
{
    public interface IRegisterService
    {
        public Task Register(User user);
    }
}
