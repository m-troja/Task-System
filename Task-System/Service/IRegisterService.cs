using Task_System.Model.Entity;
using Task_System.Model.Request;

namespace Task_System.Service
{
    public interface IRegisterService
    {
        public Task Register(RegistrationRequest rr);
    }
}
