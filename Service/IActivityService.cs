using Task_System.Model.Entity;
using Task_System.Model.IssueFolder;

namespace Task_System.Service;

public interface IActivityService
{
    Task NewCreateIssueActivity(Issue issue);
    Task NewAssignIssueActivity(User oldAssignee, Issue issue);
    Task NewSetStatusActivity(Issue issue);
    Task NewSetPriorityActivity(Issue issue);
    Task NewSetDueDateActivity(Issue issue);
}
