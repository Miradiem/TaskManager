namespace Api.Features.Tasks.GetAll;

public sealed record GetTasksRequest(
    Common.TaskStatus? Status = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    int Page = 1,
    int PageSize = 10
);