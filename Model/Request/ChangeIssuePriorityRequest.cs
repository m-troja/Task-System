﻿namespace Task_System.Model.Request;

public record ChangeIssuePriorityRequest(int IssueId, string NewPriority)
{
}
