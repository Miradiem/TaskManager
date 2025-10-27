using FluentAssertions;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.TaskTests;

public class DeleteTaskTests
{
    [Fact]
    public async Task DeleteTask_ShouldRemove_WhenExists()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var initialCount = db.Tasks.Count();
        var task = db.Tasks.First();

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();

        db.Tasks.Should().HaveCount(initialCount - 1);
        db.Tasks.Should().NotContain(t => t.Id == task.Id);
    }

    [Fact]
    public async Task DeleteTask_ShouldNotAffectOtherTasks()
    {
        using var db = TestDbContextFactory.Create();
        var userId = FakeDataSeeder.SeedUserWithTasks(db);

        var tasks = db.Tasks.Take(2).ToList();
        var taskToDelete = tasks[0];
        var taskToKeep = tasks[1];

        db.Tasks.Remove(taskToDelete);
        await db.SaveChangesAsync();

        db.Tasks.Should().Contain(t => t.Id == taskToKeep.Id);
        db.Tasks.Should().NotContain(t => t.Id == taskToDelete.Id);
    }
}