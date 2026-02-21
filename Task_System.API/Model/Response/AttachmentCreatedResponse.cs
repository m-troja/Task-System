using Task_System.Model.IssueFolder;

namespace Task_System.Model.Response;

public record AttachmentCreatedResponse(
    ResponseType responseType, 
    int fileId,
    int commentId
)
{}
