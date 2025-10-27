using Api.Features.Tasks.Common;
using TaskStatus = Api.Features.Tasks.Common.TaskStatus;

namespace Api.Features.Tasks.Update;

public sealed record UpdateTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime? DueDate,
    DateTime UpdatedAt
);