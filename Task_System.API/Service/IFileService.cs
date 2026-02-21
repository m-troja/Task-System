using Task_System.Model.IssueFolder;
using Task_System.Model.Request;

namespace Task_System.Service
{
    public interface IFileService
{
        Task<int> SaveImageAsync(FileUploadRequest req);
        Task DeleteFileAsync(int id);
        Task<CommentAttachment>? GetFileById(int id);
    }
}
