using Api.Features.Auth.Common;

namespace Api.Features.Tasks.Common;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.New;
    public DateTime? DueDate { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public enum TaskStatus
{
    New = 0,
    InProgress = 1,
    Done = 2
}
