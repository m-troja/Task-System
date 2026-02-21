namespace Task_System.Model.Request;

public record FileUploadRequest // form
    (
    
    IFormFile File,
        string CommentId
     )
{
}
