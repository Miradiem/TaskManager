using Api.Common.Persistence;
using Api.Features.Auth.Common;
using Api.Features.Tasks.Common;

namespace TaskManager.Tests.Common;

public static class FakeDataSeeder
{
    public static Guid SeedUserWithTasks(AppDbContext db)
    {
        var userId = Guid.NewGuid();
        var passwordHash = TestDbContextFactory.HashPassword("Password123!");
        
        db.Users.Add(new User 
        { 
            Id = userId, 
            Email = "test@example.com",
            PasswordHash = passwordHash,
            FirstName = "Test",
            LastName = "User"
        });
        
        db.Tasks.AddRange(
            new TaskItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Task 1", 
                UserId = userId, 
                Status = Api.Features.Tasks.Common.TaskStatus.New
            },
            new TaskItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Task 2", 
                UserId = userId, 
                Status = Api.Features.Tasks.Common.TaskStatus.Done
            }
        );
        
        db.SaveChanges();
        return userId;
    }

    public static Guid SeedUser(AppDbContext db, string email = "test@example.com", string password = "Password123!")
    {
        var userId = Guid.NewGuid();
        var passwordHash = TestDbContextFactory.HashPassword(password);
        
        db.Users.Add(new User 
        { 
            Id = userId, 
            Email = email,
            PasswordHash = passwordHash,
            FirstName = "Test",
            LastName = "User"
        });
        
        db.SaveChanges();
        return userId;
    }
}