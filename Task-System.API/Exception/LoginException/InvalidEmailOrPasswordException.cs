namespace Task_System.Exception.LoginException;

public class InvalidEmailOrPasswordException(string message) : System.Exception(message)
{
}
