using Microsoft.EntityFrameworkCore;
using Api.Common.Persistence;
using Api.Common.Jwt;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace Api.Features.Auth.Login;

public static class LoginEndpoint
{
    public static async Task<IResult> Handle(
        LoginRequest request,
        AppDbContext context,
        IJwtService jwtService,
        IValidator<LoginRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return Results.Unauthorized();
        }

        var token = jwtService.GenerateToken(user.Id.ToString(), user.Email);

        var response = new LoginResponse(
            token,
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName
        );

        return Results.Ok(response);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHash = Convert.ToBase64String(hashedBytes);
        return computedHash == passwordHash;
    }
}