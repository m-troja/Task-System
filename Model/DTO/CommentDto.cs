﻿namespace Task_System.Model.DTO;

public record CommentDto(int Id, int IssueId, string Content, int AuthorId, DateTime CreatedAt)
{
}
