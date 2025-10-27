using Api.Common.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Api.Features.Auth.Register;

public static class RegisterEndpoint
{
    public static async Task<IResult> Handle(
        RegisterRequest request,
        AppDbContext context,
        IValidator<RegisterRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser is not null)
        {
            return Results.Conflict(new { message = "User with this email already exists" });
        }

        var passwordHash = HashPassword(request.Password);

        var user = new Common.User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var response = new RegisterResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAt
        );

        return Results.Created($"/users/{user.Id}", response);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}