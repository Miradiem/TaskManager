using Api.Features.Tasks.Common;

namespace Api.Features.Tasks.Create;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate = null
);