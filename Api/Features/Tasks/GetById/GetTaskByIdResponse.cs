using Api.Features.Tasks.Common;
using TaskStatus = Api.Features.Tasks.Common.TaskStatus;

namespace Api.Features.Tasks.GetById;

public sealed record GetTaskByIdResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);