namespace Api.Features.Tasks.Create;

public sealed record CreateTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    Common.TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt
);