namespace Api.Features.Tasks.GetAll;

public sealed record GetTasksResponse(
    List<TaskDto> Tasks,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    Common.TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);