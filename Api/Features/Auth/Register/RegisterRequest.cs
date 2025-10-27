namespace Api.Features.Auth.Register;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);