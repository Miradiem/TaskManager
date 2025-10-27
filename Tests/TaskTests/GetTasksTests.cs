using FluentAssertions;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.TaskTests;

public class GetTasksTests
{
    [Fact]
    public void GetTasks_ShouldFilterByStatus()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var completedTasks = db.Tasks.Where(t => t.Status == Api.Features.Tasks.Common.TaskStatus.Done && t.UserId == userId).ToList();

        completedTasks.Should().HaveCount(1);
        completedTasks.First().Status.Should().Be(Api.Features.Tasks.Common.TaskStatus.Done);
        completedTasks.First().Title.Should().Be("Task 2");
    }

    [Fact]
    public void GetTasks_ShouldReturnAllTasks_ForUser()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var allTasks = db.Tasks.Where(t => t.UserId == userId).ToList();

        allTasks.Should().HaveCount(2);
    }
}