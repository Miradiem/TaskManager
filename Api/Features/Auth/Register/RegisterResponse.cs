namespace Api.Features.Auth.Register;

public sealed record RegisterResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);