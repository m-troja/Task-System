using Task_System.Model.IssueFolder;

namespace Task_System.Service;
    public interface ISlackNotificationService
{
        Task SendIssueCreatedNotificationAsync(Issue issue);
        Task SendIssueAssignedNotificationAsync(Issue issue);
        Task SendIssueStatusChangedNotificationAsync(Issue issue);
        Task SendIssuePriorityChangedNotificationAsync(Issue issue);
        Task SendIssueDueDateUpdatedNotificationAsync(Issue issue);
}
