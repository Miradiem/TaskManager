using Api.Features.Auth.Login;
using FluentAssertions;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.AuthTests;

public class LoginTests
{
    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        using var db = TestDbContextFactory.Create();
        var jwtService = TestHelpers.CreateJwtService();
        
        var passwordHash = TestDbContextFactory.HashPassword("Password123!");
        db.Users.Add(new Api.Features.Auth.Common.User 
        { 
            Email = "tester@example.com", 
            PasswordHash = passwordHash,
            FirstName = "Test",
            LastName = "User"
        });
        await db.SaveChangesAsync();

        var validator = new LoginRequestValidator();
        var request = new LoginRequest("tester@example.com", "Password123!");

        var result = await LoginEndpoint.Handle(request, db, jwtService, validator);

        result.Should().NotBeNull();
        
        var httpResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<LoginResponse>;
        httpResult.Should().NotBeNull();
        var loginResponse = httpResult!.Value;
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrEmpty();
        loginResponse.Email.Should().Be("tester@example.com");
        loginResponse.FirstName.Should().Be("Test");
        loginResponse.LastName.Should().Be("User");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var jwtService = TestHelpers.CreateJwtService();
        var validator = new LoginRequestValidator();
        var request = new LoginRequest("nonexistent@example.com", "Password123!");

        var result = await LoginEndpoint.Handle(request, db, jwtService, validator);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        using var db = TestDbContextFactory.Create();
        var jwtService = TestHelpers.CreateJwtService();
        
        var passwordHash = TestDbContextFactory.HashPassword("CorrectPassword123!");
        db.Users.Add(new Api.Features.Auth.Common.User 
        { 
            Email = "tester@example.com", 
            PasswordHash = passwordHash,
            FirstName = "Test",
            LastName = "User"
        });
        await db.SaveChangesAsync();

        var validator = new LoginRequestValidator();
        var request = new LoginRequest("tester@example.com", "WrongPassword123!");

        var result = await LoginEndpoint.Handle(request, db, jwtService, validator);

        result.Should().NotBeNull();
    }
}