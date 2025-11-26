using Task_System.Model.IssueFolder;

namespace Task_System.Model.Response;

public record IssueCreatedResponse(
    ResponseType responseType, 
    string key
){}
