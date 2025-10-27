using Api.Features.Auth.Register;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.AuthTests;

public class RegisterTests
{
    [Fact]
    public async Task Register_ShouldCreateUser_WhenValid()
    {
        using var db = TestDbContextFactory.Create();
        var validator = new RegisterRequestValidator();
        
        var request = new RegisterRequest(
            "newuser@example.com",
            "Password123!",
            "New",
            "User"
        );

        var result = await RegisterEndpoint.Handle(request, db, validator);

        result.Should().NotBeNull();
        
        var httpResult = result as Microsoft.AspNetCore.Http.HttpResults.Created<RegisterResponse>;
        httpResult.Should().NotBeNull();
        var registerResponse = httpResult!.Value;
        registerResponse.Should().NotBeNull();
        registerResponse!.Email.Should().Be("newuser@example.com");
        registerResponse.FirstName.Should().Be("New");
        registerResponse.LastName.Should().Be("User");
        
        var users = await db.Users.ToListAsync();
        users.Should().ContainSingle(u => u.Email == "newuser@example.com");
        users.First().FirstName.Should().Be("New");
        users.First().LastName.Should().Be("User");
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        using var db = TestDbContextFactory.Create();
        FakeDataSeeder.SeedUser(db, "existing@example.com");
        var validator = new RegisterRequestValidator();
        
        var request = new RegisterRequest(
            "existing@example.com",
            "Password123!",
            "Another",
            "User"
        );

        var result = await RegisterEndpoint.Handle(request, db, validator);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("Conflict");
    }
}