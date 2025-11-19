using Task_System.Model.Entity;

namespace Task_System.Service;

public interface IChatGptService
{
    Task<List<User>> RegisterSlackUsers(List<ChatGptUser> chatGptUser);
    Task<List<User>> GetAllChatGptUsersAsync();
}
