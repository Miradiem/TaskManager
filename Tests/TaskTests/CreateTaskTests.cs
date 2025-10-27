using Api.Features.Tasks.Common;
using FluentAssertions;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.TaskTests;

public class CreateTaskTests
{
    [Fact]
    public async Task CreateTask_ShouldPersist_WhenValid()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var newTask = new TaskItem 
        { 
            Title = "New Task", 
            Description = "Test Description",
            UserId = userId,
            Status = Api.Features.Tasks.Common.TaskStatus.New
        };

        db.Tasks.Add(newTask);
        await db.SaveChangesAsync();

        db.Tasks.Should().Contain(t => t.Title == "New Task");
        db.Tasks.Should().HaveCount(3);
    }
}