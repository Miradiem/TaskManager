using FluentAssertions;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.TaskTests;

public class UpdateTaskTests
{
    [Fact]
    public async Task UpdateTask_ShouldModifyStatus_WhenExists()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var task = db.Tasks.First();
        var originalStatus = task.Status;

        task.Status = Api.Features.Tasks.Common.TaskStatus.InProgress;
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        db.Tasks.First().Status.Should().Be(Api.Features.Tasks.Common.TaskStatus.InProgress);
        db.Tasks.First().Status.Should().NotBe(originalStatus);
    }

    [Fact]
    public async Task UpdateTask_ShouldModifyTitle_WhenExists()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var task = db.Tasks.First();

        task.Title = "Updated Task Title";
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        db.Tasks.First().Title.Should().Be("Updated Task Title");
    }
}